using UnityEngine;
using System.Collections;
using ModestTree.Zenject;

#if UNITY_EDITOR && !UNITY_WEBPLAYER
using Moq;
#endif

namespace ModestTree
{
    public static class ZenjectMoqExtensions
    {
        public static BindingConditionSetter ToMock<TContract>(this ReferenceBinder<TContract> binder) where TContract : class
        {
#if UNITY_EDITOR && !UNITY_WEBPLAYER
            return binder.To(new SingletonInstanceProvider(Mock.Of<TContract>()));
#else
            Assert.That(false, "The use of 'ToMock' in web builds is not supported");
            return null;
#endif
        }
    }
}
