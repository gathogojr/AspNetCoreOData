//-----------------------------------------------------------------------------
// <copyright file="MockType.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moq;
using Moq.Protected;

namespace Microsoft.AspNetCore.OData.TestCommon;

/// <summary>
/// A mock to represent a CLR type.
/// </summary>
public sealed class MockType : Mock<Type>
{
    private readonly List<MockPropertyInfo> _propertyInfos = new List<MockPropertyInfo>();
    private MockType _baseType;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockType"/> class.
    /// </summary>
    public MockType()
        : this("T")
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MockType"/> class.
    /// </summary>
    /// <param name="typeName">The type name.</param>
    /// <param name="hasDefaultCtor">Has default constructor.</param>
    /// <param name="namespace">The namespace.</param>
    public MockType(string typeName, bool hasDefaultCtor = true, string @namespace = "DefaultNamespace")
    {
        SetupGet(t => t.Name).Returns(typeName);
        SetupGet(t => t.BaseType).Returns(typeof(Object));
        SetupGet(t => t.Assembly).Returns(typeof(object).Assembly);
        Setup(t => t.GetProperties(It.IsAny<BindingFlags>()))
            .Returns(() => _propertyInfos.Union(_baseType != null ? _baseType._propertyInfos : Enumerable.Empty<MockPropertyInfo>()).Select(p => p.Object).ToArray());
        Setup(t => t.Equals(It.IsAny<object>())).Returns<Type>(t => ReferenceEquals(Object, t));
        Setup(t => t.ToString()).Returns(typeName);
        Setup(t => t.Namespace).Returns(@namespace);
        Setup(t => t.IsAssignableFrom(It.IsAny<Type>())).Returns(true);
        Setup(t => t.FullName).Returns(@namespace + "." + typeName);

        TypeAttributes(System.Reflection.TypeAttributes.Class | System.Reflection.TypeAttributes.Public);


        if (hasDefaultCtor)
        {
            this.Protected()
                .Setup<ConstructorInfo>(
                    "GetConstructorImpl",
                    BindingFlags.Instance | BindingFlags.Public,
                    ItExpr.IsNull<Binder>(),
                    CallingConventions.Standard | CallingConventions.VarArgs,
                    Type.EmptyTypes,
                    ItExpr.IsNull<ParameterModifier[]>())
                .Returns(new Mock<ConstructorInfo>().Object);
        }
    }

    /// <summary>
    /// Implicit operator to convert Mock type info to type.
    /// </summary>
    /// <param name="mockType">The mock type.</param>
    public static implicit operator Type(MockType mockType)
    {
        return mockType.Object;
    }

    public MockType TypeAttributes(TypeAttributes typeAttributes)
    {
        this.Protected()
            .Setup<TypeAttributes>("GetAttributeFlagsImpl")
            .Returns(typeAttributes);

        return this;
    }

    public MockType BaseType(MockType mockBaseType)
    {
        _baseType = mockBaseType;
        SetupGet(t => t.BaseType).Returns(mockBaseType);
        Setup(t => t.IsSubclassOf(mockBaseType)).Returns(true);

        return this;
    }

    public MockType Property<T>(string propertyName)
    {
        Property(typeof(T), propertyName);

        return this;
    }

    public MockType Property(Type propertyType, string propertyName, params Attribute[] attributes)
    {
        var mockPropertyInfo = new MockPropertyInfo(propertyType, propertyName);
        mockPropertyInfo.SetupGet(p => p.DeclaringType).Returns(this);
        mockPropertyInfo.SetupGet(p => p.ReflectedType).Returns(this);
        mockPropertyInfo.Setup(p => p.GetCustomAttributes(It.IsAny<bool>())).Returns(attributes);

        _propertyInfos.Add(mockPropertyInfo);

        return this;
    }

    public MockPropertyInfo GetProperty(string name)
    {
        return _propertyInfos.Single(p => p.Object.Name == name);
    }

    public MockType AsCollection()
    {
        var mockCollectionType = new MockType();

        mockCollectionType.Setup(t => t.GetInterfaces()).Returns(new Type[] { typeof(IEnumerable<>).MakeGenericType(this) });

        return mockCollectionType;
    }
}
