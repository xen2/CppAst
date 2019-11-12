// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Linq;
using NUnit.Framework;

namespace CppAst.Tests
{
    public class TestTemplates : InlineTestBase
    {
        [Test]
        public void TestSimple()
        {
            ParseAssert(@"
class Test1 { void Method1(); };
template <class T> class Test2 { void Method1(); };
class Test3 { void Method1(Test1* test1, Test2<int*> param1); };
",
                compilation =>
                {
                    Assert.False(compilation.HasErrors);

                    Assert.AreEqual(5, compilation.Fields.Count);
                    Assert.AreEqual(2, compilation.Typedefs.Count);

                    var types = new CppType[]
                    {
                        new CppReferenceType(CppPrimitiveType.Int) ,
                        new CppQualifiedType(CppTypeQualifier.Const, CppPrimitiveType.Float),

                        new CppPointerType(CppPrimitiveType.Char),
                        new CppQualifiedType(CppTypeQualifier.Const, CppPrimitiveType.Int),
                        new CppArrayType(CppPrimitiveType.Int, 5),
                        new CppPointerType(new CppFunctionType(CppPrimitiveType.Void)
                        {
                            Parameters =
                            {
                                new CppParameter(CppPrimitiveType.Int, "a"),
                                new CppParameter(CppPrimitiveType.Float, "b"),
                            }
                        }) { SizeOf = IntPtr.Size },
                        new CppPointerType(new CppQualifiedType(CppTypeQualifier.Const, CppPrimitiveType.Float))
                    };

                    var canonicalTypes = compilation.Typedefs.Select(x => x.GetCanonicalType()).Concat(compilation.Fields.Select(x => x.Type.GetCanonicalType())).ToList();
                    Assert.AreEqual(types, canonicalTypes);
                    Assert.AreEqual(types.Select(x => x.SizeOf), canonicalTypes.Select(x => x.SizeOf));
                }
            );
        }
    }
}