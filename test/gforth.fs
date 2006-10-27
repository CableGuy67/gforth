\ test some gforth extension words

\ Copyright (C) 2003,2004,2005 Free Software Foundation, Inc.

\ This file is part of Gforth.

\ Gforth is free software; you can redistribute it and/or
\ modify it under the terms of the GNU General Public License
\ as published by the Free Software Foundation; either version 2
\ of the License, or (at your option) any later version.

\ This program is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
\ GNU General Public License for more details.

\ You should have received a copy of the GNU General Public License
\ along with this program; if not, write to the Free Software
\ Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111, USA.

require ./tester.fs
decimal

\ f>str-rdp (then f.rdp and f>buf-rdb should also be ok)

{  12.3456789e 7 3 1 f>str-rdp s"  12.346" str= -> true }
{  12.3456789e 7 4 1 f>str-rdp s" 12.3457" str= -> true }
{ -12.3456789e 7 4 1 f>str-rdp s" -1.23E1" str= -> true }
{      0.0996e 7 3 1 f>str-rdp s"   0.100" str= -> true }
{      0.0996e 7 3 3 f>str-rdp s" 9.96E-2" str= -> true }
{    999.9994e 7 3 1 f>str-rdp s" 999.999" str= -> true }
{    999.9996e 7 3 1 f>str-rdp s" 1.000E3" str= -> true }
{       -1e-20 5 2 1 f>str-rdp s" *****"   str= -> true }

\ 0x hex number conversion, or not

decimal
{ 0x10 -> 16 }
{ 0X10 -> 16 }
36 base !
{ 0x10 -> x10 }
decimal
{ 'a' -> 97 }
{ 'A  -> 65 }
{ 1. '1 -> 1. 49 }

\ represent has no trailing 0s even for inf and nan

{  1e 0e f/ pad 16 represent drop 2drop pad 15 + c@ '0 = -> false }
{  0e 0e f/ pad 16 represent drop 2drop pad 15 + c@ '0 = -> false }
{ -1e 0e f/ pad 16 represent drop 2drop pad 15 + c@ '0 = -> false }

\ gforth now guarantees exceptions in division errors

\ division by zero
{ 1 0 ' /    catch -> 1 0 -10 }
{ 1 0 ' mod  catch -> 1 0 -10 }
{ 1 0 ' /mod catch -> 1 0 -10 }
{ 1 1 0 ' */mod catch -> 1 1 0 -10 }
{ 1 1 0 ' */    catch -> 1 1 0 -10 }
{ 1. 0 ' fm/mod catch -> 1. 0 -10 }
{ 1. 0 ' sm/rem catch -> 1. 0 -10 }
{ 1. 0 ' um/mod catch -> 1. 0 -10 }

\ division overflows (might come out as "division by zero" or "overflow")
environment-wordlist >order
{ 1 1 dnegate 2 ' sm/rem catch 0= -> -1 max-n invert true }

{ 1 1 -2 ' sm/rem catch 0= -> 1 max-n invert true }

{ max-u max-n 2/ max-n invert ' fm/mod catch -> -1 max-n invert 0 }
{ max-u max-n 2/ max-n invert ' sm/rem catch -> max-n max-n negate 0 }

{ 0 max-n 2/ 1+ max-n invert ' fm/mod catch -> 0 max-n invert 0 }
{ 0 max-n 2/ 1+ max-n invert ' sm/rem catch -> 0 max-n invert 0 }

{ 1 max-n 2/ 1+ max-n invert ' sm/rem catch 0= -> 1 max-n invert true }

{ 0 max-u -1. d+ max-u ' um/mod catch 0= -> max-u 1- max-u true }
