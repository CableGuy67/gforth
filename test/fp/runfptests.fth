\ To run Floating Point tests

cr .( Running FP Tests) cr

s" [undefined]" pad c! pad char+ pad c@ move 
pad find nip 0=
[if]
   : [undefined]  ( "name" -- flag )
      bl word find nip 0=
   ; immediate
[then]
[undefined] warnings 0= [if] warnings off [then] \ warnings clutter output

s" ttester.fs"         included
s" ak-fp-test.fth"     included
s" fatan2-test.fs"     included
s" ieee-arith-test.fs" included
s" ieee-fprox-test.fs" included
s" fpzero-test.4th"    included
s" fpio-test.4th"      included
s" to-float-test.4th"  included
s" paranoia.4th"       included

cr cr 
.( FP tests finished)
#errors @ 0= [IF] .(  successfully) [ELSE] .(  failed ) #errors ? .( times) cr ABORT [THEN]
cr cr