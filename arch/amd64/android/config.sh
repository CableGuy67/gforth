# First create a standalone toolchain directory
#mkdir ~/proj/android-toolchain-x86_64
#cd  ~/proj/android-toolchain-x86_64
#~/proj/android-ndk-r10e/build/tools/make-standalone-toolchain.sh --platform=android-21 --ndk-dir=/home/bernd/proj/android-ndk-r10e --install-dir=$PWD --toolchain=x86_64-linux-android-4.9
#configure with
#./configure --host=x86_64-linux-android --with-cross=android --prefix= --datarootdir=/sdcard --libdir=/sdcard --libexecdir=/lib --enable-lib --with-ditc=gforth-ditc
#and finally create an apk in this directory
#./build.sh
echo "Config for android-amd64"

CC=x86_64-linux-android-gcc
TOOLCHAIN=$(which $CC | sed -e s,/bin/.*-gcc,,g)

XLIBS="sigaltstack.o __set_errno.o" # sigemptyset.o sigaddset.o termios.o"
(mkdir -p engine/.libs
 cd engine
 for i in $XLIBS
 do
     ar x $TOOLCHAIN/sysroot/usr/lib64/libc.a $i
     cp $i .libs/lib$i
     cp $i lib$i
     echo "# lib${i%o}lo - a libtool object file
# Generated by libtool (GNU libtool) 2.4.2
#
# Please DO NOT delete this file!
# It is necessary for linking the library.

# Name of the PIC object.
pic_object='.libs/lib$i'

# Name of the non-PIC object
non_pic_object='lib$i'" >lib${i%o}lo
 done
)
skipcode=".skip 16"
kernel_fi=kernl64l.fi
ac_cv_sizeof_void_p=8
ac_cv_sizeof_char_p=8
ac_cv_sizeof_char=1
ac_cv_sizeof_short=2
ac_cv_sizeof_int=4
ac_cv_sizeof_long=8
ac_cv_sizeof_long_long=8
ac_cv_sizeof_intptr_t=8
ac_cv_sizeof_int128_t=16
ac_cv_c_bigendian=no
ac_cv_func_memcmp_working=yes
ac_cv_func_memmove=yes
ac_cv_func_getpagesize=no
ac_cv_func_wcwidth=no
ac_cv_file___arch_amd64_asm_fs=yes
ac_cv_file___arch_amd64_disasm_fs=yes
ac_cv_func_dlopen=yes
ac_cv_lib_ltdl_lt_dlinit=no
ac_export_dynamic=no
HOSTCC="gcc -m64"
GNU_LIBTOOL="x86_64-linux-android-libtool"
LIBTOOL="x86_64-linux-android-libtool"
build_libcc_named=build-libcc-named
extraccdir=/data/app/gnu.gforth-1/lib/x86_64
asm_fs=arch/amd64/asm.fs
disasm_fs=arch/amd64/disasm.fs
EC_MODE="false"
EXTRAPREFIX="\$(shell which \$(GCC) | sed -e s,/bin/\$(GCC),/sysroot/usr,g)"
NO_EC=""
EC=""
engine2='engine2$(OPT).o'
engine_fast2='engine-fast2$(OPT).o'
no_dynamic=""
image_i=""
LIBS="-llog -lz"
signals_o="io.o signals.o $XLIBS androidmain.o zexpand.o"
GFORTH=`which gforth-amd64`