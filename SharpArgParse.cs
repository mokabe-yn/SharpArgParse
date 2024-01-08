/* SharpArgParse v0.9.1
 * Copyright (c) 2024 mokabe-yn <okabe_m@hmi.aitech.ac.jp>
 * 
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
 * WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. * 
 * 
 * */

//#define ARGPARCE_EXPORT
//#define ARGPARCE_BACKPORT_NET40
//#define ARGPARCE_BACKPORT_NET45
//#define ARGPARCE_BACKPORT_SHARP7_3

// Note: This library supports for C#7.3 and .NET Framework 4.7.2 environs.
//       This library use only C#7.3 and .NET Framework 4 features.
//       requires special attentions for cannot uses ...
//       * Span (Requires External Library at .NET Framework 4.x)
//       * unsafe (force users to allow unsafe context)
//       * Nullable class
//         This library requires legal C# 7.3 and C#8.0 syntax.
//         `string Value;` is always notnull. (by C#8.0)
//         But user inputs maybe nullable. (by C#7.3)
//         Cannot annotate nullable like `string? Value`.
//       * ValueTuple (only net47 or later)

// TODO: auto generate help.
// TODO: call help-mode: prog.exe --help

#pragma warning disable IDE0290 // C#12: primary constructor
#pragma warning disable CA1510  // C#10: ArgumentNullException.ThrowIfNull
#pragma warning disable IDE0251 // C#8: readonly instance members
#pragma warning disable IDE0090 // C#9: target = new();
#pragma warning disable IDE0056 // C#8: Index (array[^1]);
#pragma warning disable IDE0057 // C#8: Range (array[4..8]);


using System;
using System.Collections.Generic;
using System.Reflection;
using SharpArgParse.Internals;

namespace SharpArgParse.Internals
{

}

namespace SharpArgParse
{


}
