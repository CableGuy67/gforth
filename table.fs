\ table fomerly in search.fs

\ Authors: Anton Ertl, Bernd Paysan, Jens Wilke, Neal Crook
\ Copyright (C) 1996,1997,1999,2001,2003,2007,2015,2017,2019 Free Software Foundation, Inc.

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

require hash.fs

\ table (case-sensitive wordlist)

: table-find ( addr len wordlist -- nfa / false )
    >r 2dup r> bucket @ (tablelfind) ;
: table-rec ( addr len wordlist-id -- nfa rectype-nt / rectype-null )
    0 wordlist-id - table-find nt>rec ;

Create tablesearch-map ( -- wordlist-map )
0 , 0 , ' table-reveal A, ' (rehash) A, ' table-rec A,
Create cs-wordlist-search-map ( -- wordlist-map )
0 , 0 , ' hash-reveal A,  ' (rehash) A, ' table-rec A,

: table ( -- wid ) \ gforth
    \g Create a lookup table (case-sensitive, no warnings).
    tablesearch-map mappedwordlist ;

: cs-wordlist ( -- wid ) \ gforth
    \g Create a case-sensitive wordlist.
    cs-wordlist-search-map mappedwordlist ;

: cs-vocabulary ( "name" -- ) \ gforth
    \g Create a case-senisitve vocabulary
    Vocabulary cs-wordlist-search-map latestnt >body ! ;

' cs-vocabulary alias voctable
