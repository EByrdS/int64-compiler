# int64 compiler, version 0.1

This program is free software. You may redistribute it under the terms of
the GNU General Public License version 3 or later. See license.txt for
details.

Included in this release:

- Lexical analysis
- Syntactic analysis
- Semantic analysis

## Requirements

You will need a C# compiler, mainly `mono`. The makefile uses `mcs` to transform `.cs`
files to `.exe` binaries.

## Running the compiler

First, run:

`make int64lib.dll`

Then, to build the compiler, run:

`make int641.exe` or just `make`.

To compile an `int64` file to `CIL`, run:

`./int64.exe <file_name>`

Where <file_name> is the name of a int64 source file. You can try with
these files under `int64_programs`:

- hello.int64
- palindrome.int64
- binary.int64

This will generate a `.il` file with CIL code (Common Intermediate Language).
You will need to turn that file into an executable before you can run it.

### Update: 11-Sept-2022

ILasm seems to be the tool needed to convert `.il` files to `.exe`.
However, those programs are currently
not running, maybe because the `.exe` files are built for Linux or Windows, but I
am running on a Mac.
