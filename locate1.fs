\ SwiftForth-like locate etc.

\ Copyright (C) 2016 Free Software Foundation, Inc.

\ This file is part of Gforth.

\ Gforth is free software; you can redistribute it and/or
\ modify it under the terms of the GNU General Public License
\ as published by the Free Software Foundation, either version 3
\ of the License, or (at your option) any later version.

\ This program is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
\ GNU General Public License for more details.

\ You should have received a copy of the GNU General Public License
\ along with this program. If not, see http://www.gnu.org/licenses/.

: set-bn-xpos ( -- )
    bn-xpos @ xpos>file# located-top @ 0 encode-pos1 bn-xpos ! ;

: slurp-located ( -- )
    located-slurped 2@ drop 0= if
	located-xpos @ xpos>file# loadfilename#>str slurp-file
	located-slurped 2!
    then ;

: locate-line {: c-addr1 u1 lineno -- c-addr2 u2 lineno+1 c-addr1 u3 :}
    \ c-addr1 u1 is the rest of the file, c-addr1 u3 the line, and
    \ c-addr2 u2 the rest of the file without the line
    u1 0 u+do
	c-addr1 u1 i /string s\" \r\l" string-prefix? if
	    c-addr1 u1 i 2 + /string lineno 1+ c-addr1 i unloop exit then
	c-addr1 i + c@ dup #lf = swap #cr = or if
	    c-addr1 u1 i 1 + /string lineno 1+ c-addr1 i unloop exit then
    loop
    c-addr1 u1 + 0 lineno 1+ c-addr1 u1 ;

: locate-next-line ( c-addr1 u1 lineno -- c-addr2 u2 lineno+1 )
    locate-line 2drop ;

: type-prefix ( c-addr1 u1 u -- c-addr2 u2 )
    \ type the u-len prefix of c-addr1 u1, c-addr2 u2 is the rest
    >r 2dup r> umin tuck type /string ;

: locate-type ( c-addr u lineno -- )
    cr located-xpos @ xpos>line = if
	warn-color attr! located-xpos @ xpos>char type-prefix
	err-color  attr! located-len @            type-prefix
	warn-color attr! type
	default-color attr! exit
    then
    type ;

: locate-print-line ( c-addr1 u1 lineno -- c-addr2 u2 lineno+1 )
    dup >r locate-line r> locate-type ;

: l1 ( -- )
    slurp-located
    located-slurped 2@ 1 case ( c-addr u lineno1 )
	over 0= ?of endof
	dup located-bottom @ >= ?of endof
	dup located-top @ >= ?of locate-print-line contof
	locate-next-line
    next-case
    2drop drop ;

: l ( -- )
    \g Display line of source after compiler error or locate
    cr located-xpos @ xpos>file# loadfilename#>str type  ': emit
    located-top @ dec.
    l1 ;

: name-set-located-xpos ( nt -- )
    dup name>view @ swap name>string nip set-located-xpos ;

: locate-name ( nt -- )
     name-set-located-xpos l ;

: locate ( "name" -- )
    (') locate-name ;

: n ( -- )
    \g Display next lines after locate or error
    located-bottom @ dup located-top ! form drop 2/ + located-bottom !
    set-bn-xpos l1 ;

: b ( -- )
    \g Display previous lines after locate.
    located-top @ dup located-bottom ! form drop 2/ - 0 max located-top !
    set-bn-xpos l ;

: g ( -- )
    \g Enter the editor at the place of the latest error, @code{locate},
    \g @code{n} or @code{b}.
    bn-xpos @ ['] editor-cmd >string-execute 2dup system drop free throw ;

: edit ( "name" -- )
    \g Enter the editor at the place of "name"
    (') name-set-located-xpos g ;


\ backtrace locate stuff:

\ an alternative implementation of much of this stuff is elsewhere.
\ The following implementation works for code in sections, too, but
\ currently does not survive SAVESYSTEM.
0 [if]
256 1024 * constant bl-data-size

0
2field:  bl-bounds
field:   bl-next
bl-data-size cell+ +field bl-data
constant bl-size

variable code-locations 0 code-locations !

: .bl {: bl -- :}
    cr bl bl-bounds 2@ swap 16 hex.r 17 hex.r
    bl bl-data 17 hex.r
    bl bl-next @ 17 hex.r ;

: .bls ( -- )
    cr ."       code-start         code-end          bl-data          bl-next"
    code-locations @ begin
	dup while
	    dup .bl
	    bl-next @ repeat
    drop ;

: addr>pos1 ( addr -- pos1|0 )
    code-locations @ begin ( addr bl )
	dup while
	    2dup bl-bounds 2@ within if
		tuck bl-bounds 2@ drop  - + bl-data @ exit then
	    bl-next @ repeat
    2drop 0 ;

: xt-location2 ( addr bl -- addr )
    \ knowing that addr is within bl, record the current source
    \ position for addr
    2dup bl-bounds 2@ drop - + bl-data ( addr addr' )
    current-sourcepos1 swap 2dup ! cell+ ! ;

: new-bl ( addr blp -- )
    bl-size allocate throw >r
    swap dup bl-data-size + r@ bl-bounds 2!
    dup @ r@ bl-next !
    r@ bl-data bl-data-size cell+ erase
    r> swap ! ;
    
: xt-location1 ( addr -- addr )
    code-locations begin ( addr blp )
	dup @ 0= if
	    2dup new-bl then
	@ 2dup bl-bounds 2@ within 0= while ( addr bl )
	    bl-next repeat
    xt-location2 ;

' xt-location1 is xt-location
[then]

: bt-location ( u -- f )
    \ locate-setup backtrace entry with index u; returns true iff successful
    cells >r stored-backtrace $@ r@ u> if ( addr1 r: offset )
	r> + @ cell- addr>pos1 dup if ( xpos )
	    1 set-located-xpos true exit then
    else
	rdrop then
    drop ." no location for this backtrace index" false ;

: lb ( u -- )
    bt-location if
	l then ;

\ where

: unbounds ( c-start c-end -- c-start u )
    over - ;
    
: .wheretype ( c-addr u xpos -- )
    xpos>char >r -trailing over r> + {: c-pos :} 2dup + {: c-lineend :} 
    (parse-white) drop ( c-addr1 )
    warn-color attr! c-pos unbounds type
    err-color  attr! c-pos c-lineend unbounds (parse-white) tuck type
    warn-color attr! c-pos + c-lineend unbounds type
    default-color attr! ;
    
: .whereline {: xpos u -- :}
    \ print the part of the source line around xpos that fits in the
    \ current line, of which u characters have already been used
    xpos xpos>file# loadfilename#>str slurp-file 1 case ( c-addr u lineno1 )
	over 0= ?of endof
	dup xpos xpos>line = ?of locate-line xpos .wheretype endof
	locate-next-line
    next-case
    drop 2drop ;

: .wherepos1 ( xpos -- )
    dup .sourcepos1-width ": " type 2 + .whereline ;

: forwheres ( ... xt -- ... )
    { xt } wheres $@ bounds u+do
	i where-nt @ xt execute if
	    i where-loc @ cr .wherepos1
	then
    where-struct +loop ;

: where ( "name" -- )
    parse-name find-name [: over = ;] forwheres drop ;

\ count word usage

: usage# ( nt -- n )
    \G count usage of the word @var{nt}
    0 wheres $@ bounds U+DO
	over i where-nt @ = -
    where-struct +LOOP  nip ;

\ display unused words

lcount-mask 1+ Constant unused-mask

: .wids ( nt1 .. ntn n ) cr 0 swap 0 ?DO swap .word LOOP drop ;
: +unused ( nt -- )
    >f+c unused-mask over @ or swap ! ;
: -unused ( nt -- )
    >f+c unused-mask invert over @ and swap ! ;
: unused-words ( -- )
    \G list all words without usage
    [: +unused true ;] context @ traverse-wordlist
    wheres $@ bounds U+DO
	i where-nt @ dup forthstart here within
	IF  -unused  ELSE  drop  THEN
    where-struct +LOOP
    0 [: dup >f+c @ unused-mask and IF
	    dup -unused swap 1+
	ELSE  drop  THEN  true ;]
    context @ traverse-wordlist .wids ;

