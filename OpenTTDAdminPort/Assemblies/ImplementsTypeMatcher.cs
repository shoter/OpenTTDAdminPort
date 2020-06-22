using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Assemblies
{
    internal class ImplementsTypeMatcher : ITypeMatcher
    {
        private Type[] genericArguments;
        private Type interfaceType;

        public ImplementsTypeMatcher(Type interfaceType, params Type[] genericArguments)
        {
            this.genericArguments = genericArguments;
            this.interfaceType = interfaceType;
        }

        public static ImplementsTypeMatcher Create<TInterface>() => new ImplementsTypeMatcher(typeof(TInterface));

        public static ImplementsTypeMatcher Create<TInterface, T1>() => new ImplementsTypeMatcher(typeof(TInterface), typeof(T1));

        public static ImplementsTypeMatcher Create<TInterface, T1, T2>() => new ImplementsTypeMatcher(typeof(TInterface), typeof(T1), typeof(T2));

        public bool IsMatching(Type type)
        {
            bool isCorrect = false;

            foreach(Type i in type.GetInterfaces())
            {
                if (!i.IsInterface)
                    continue;
                if (i != interfaceType)
                    continue;
                if(i.IsGenericType && genericArguments.Any())
                {
                    Type[] genericArguments = i.GetGenericArguments();

                    if (genericArguments.Length != genericArguments.Length)
                        break;

                    bool isCorrectGenericArguments = true;
                    for(int j = 0;j < genericArguments.Length; ++j)
                    {
                        if (genericArguments[j] != this.genericArguments[j])
                        {
                            isCorrectGenericArguments = false;
                            break;
                        }

                    }
                    if (!isCorrectGenericArguments)
                        break;
                }

                isCorrect = true;
                break;
            }

            return isCorrect;
        }
    }
}
