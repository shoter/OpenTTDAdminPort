using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Assemblies
{
    /// <summary>
    /// Checks if given type is implementing given interface.
    /// </summary>
    internal class ImplementsTypeMatcher : ITypeMatcher
    {
        private Type[] genericArguments;
        private Type interfaceType;

        /// <summary>
        /// Constructs this class
        /// </summary>
        /// <param name="interfaceType">Type of interface for which we will be checking if given type implements the interface</param>
        /// <param name="genericArguments">If <see cref="interfaceType"/> is generic interface then <see cref="genericArguments"/> will be used to check if generic arguments
        /// or interface are the same and in the same order. If empty then not used even for generic <see cref="interfaceType"/></param>
        public ImplementsTypeMatcher(Type interfaceType, params Type[] genericArguments)
        {
            this.genericArguments = genericArguments;
            this.interfaceType = interfaceType;
        }

        public static ImplementsTypeMatcher Create<TInterface>() => new ImplementsTypeMatcher(typeof(TInterface));

        public static ImplementsTypeMatcher Create<TInterface, T1>() => new ImplementsTypeMatcher(typeof(TInterface), typeof(T1));

        public static ImplementsTypeMatcher Create<TInterface, T1, T2>() => new ImplementsTypeMatcher(typeof(TInterface), typeof(T1), typeof(T2));

        public bool IsMatching(Type type) => type.GetInterfaces().Any(this.IsMatchingInterface);

        private bool IsMatchingInterface(Type interf)
        {
            if (!interf.IsInterface)
                return false;
            if (interf.GUID != interfaceType.GUID)
                return false;
            if (interf.IsGenericType && genericArguments.Any())
            {
                if (!HasCorrectGenericArguments(interf.GetGenericArguments(), this.genericArguments))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether genericArguments are of the same type as interface generic arguments.
        /// </summary>
        /// <param name="interfaceGenericArguments">The interface generic arguments.</param>
        /// <param name="genericArguments">The generic arguments.</param>
        /// <returns>
        /// False if there is different number of generic arguments compared to interface arguments or when there is type mismatch for given generic argument.
        /// </returns>
        private static bool HasCorrectGenericArguments(Type[] interfaceGenericArguments, Type[] genericArguments)
        {
            if (interfaceGenericArguments.Length != genericArguments.Length)
                return false;

            for (int j = 0; j < interfaceGenericArguments.Length; ++j)
            {
                if (interfaceGenericArguments[j].GUID != genericArguments[j].GUID)
                {
                    if (interfaceGenericArguments[j].GetInterfaces().Length == 0)
                        return false;
                    return IsImplementing(interfaceGenericArguments[j], genericArguments[j]);
                }
            }
            return true;
        }

        public static bool IsImplementing(Type who, Type what)
        {
            foreach (var i in who.GetInterfaces())
            {
                if (i.GUID == what.GUID)
                    return true;
            }
            return false;
        }

    }
}
