﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TalesGenerator.Net.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TalesGenerator.Net.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Неправильный формат..
        /// </summary>
        internal static string InvalidFormatError {
            get {
                return ResourceManager.GetString("InvalidFormatError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Невозможно создать дугу с началом, совпадающим с концом..
        /// </summary>
        internal static string NetworkEdgeCreateError {
            get {
                return ResourceManager.GetString("NetworkEdgeCreateError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Невозможно создание нескольких дуг типа is-a для одной вершины..
        /// </summary>
        internal static string NetworkIsAEdgeError {
            get {
                return ResourceManager.GetString("NetworkIsAEdgeError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Каждая вершина может иметь только одну из дуг: is-a и is-instance..
        /// </summary>
        internal static string NetworkIsInstanceEdgeError {
            get {
                return ResourceManager.GetString("NetworkIsInstanceEdgeError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Создание дуг типа is-instance допускается только для вершин класса..
        /// </summary>
        internal static string NetworkIsInstanceEdgeError2 {
            get {
                return ResourceManager.GetString("NetworkIsInstanceEdgeError2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Невозможно создать две дуги одинакового типа..
        /// </summary>
        internal static string NetworkSameEdgesError {
            get {
                return ResourceManager.GetString("NetworkSameEdgesError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string ReasonerAgentQueryError {
            get {
                return ResourceManager.GetString("ReasonerAgentQueryError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Не знаю.
        /// </summary>
        internal static string ReasonerDontKnowAnswer {
            get {
                return ResourceManager.GetString("ReasonerDontKnowAnswer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Пустой запрос..
        /// </summary>
        internal static string ReasonerEmptyQueryError {
            get {
                return ResourceManager.GetString("ReasonerEmptyQueryError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string ReasonerGoalQueryError {
            get {
                return ResourceManager.GetString("ReasonerGoalQueryError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Форма запроса &quot;S1 это S2?&quot;: &lt;имя вершины&gt; это &lt;имя вершины&gt;..
        /// </summary>
        internal static string ReasonerIsQueryError {
            get {
                return ResourceManager.GetString("ReasonerIsQueryError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Нет.
        /// </summary>
        internal static string ReasonerNoAnswer {
            get {
                return ResourceManager.GetString("ReasonerNoAnswer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string ReasonerRecipientQueryError {
            get {
                return ResourceManager.GetString("ReasonerRecipientQueryError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Форма запроса &quot;Где V?&quot;: где &lt;имя падежной рамки&gt;..
        /// </summary>
        internal static string ReasonerWhereQueryError {
            get {
                return ResourceManager.GetString("ReasonerWhereQueryError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Форма запроса &quot;Кто S?&quot;: кто &lt;имя сущ.&gt;..
        /// </summary>
        internal static string ReasonerWhoQueryError {
            get {
                return ResourceManager.GetString("ReasonerWhoQueryError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Да.
        /// </summary>
        internal static string ReasonerYesAnswer {
            get {
                return ResourceManager.GetString("ReasonerYesAnswer", resourceCulture);
            }
        }
    }
}
