﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace VirtualAssistant.Dialogs.Main.Resources {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class MainStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal MainStrings() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("VirtualAssistant.Dialogs.Main.Resources.MainStrings", typeof(MainStrings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   重写当前线程的 CurrentUICulture 属性
        ///   重写当前线程的 CurrentUICulture 属性。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 What&apos;s my schedule? 的本地化字符串。
        /// </summary>
        public static string CALENDAR_SUGGESTEDACTION {
            get {
                return ResourceManager.GetString("CALENDAR_SUGGESTEDACTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Ok, let&apos;s start over. 的本地化字符串。
        /// </summary>
        public static string CANCELLED {
            get {
                return ResourceManager.GetString("CANCELLED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 What else can I help you with? 的本地化字符串。
        /// </summary>
        public static string COMPLETED {
            get {
                return ResourceManager.GetString("COMPLETED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 I&apos;m sorry, I&apos;m not able to help with that. 的本地化字符串。
        /// </summary>
        public static string CONFUSED {
            get {
                return ResourceManager.GetString("CONFUSED", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Send an email 的本地化字符串。
        /// </summary>
        public static string EMAIL_SUGGESTEDACTION {
            get {
                return ResourceManager.GetString("EMAIL_SUGGESTEDACTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Sorry, it looks like something went wrong. 的本地化字符串。
        /// </summary>
        public static string ERROR {
            get {
                return ResourceManager.GetString("ERROR", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Hi there! 的本地化字符串。
        /// </summary>
        public static string GREETING {
            get {
                return ResourceManager.GetString("GREETING", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Hey, {0}! 的本地化字符串。
        /// </summary>
        public static string GREETING_WITH_NAME {
            get {
                return ResourceManager.GetString("GREETING_WITH_NAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 I&apos;m your Virtual Assistant! I can perform a number of tasks through my connected skills. Right now I can help you with Calendar, Email, Task and Point of Interest questions. Or you can help me do more by creating your own! 的本地化字符串。
        /// </summary>
        public static string HELP_TEXT {
            get {
                return ResourceManager.GetString("HELP_TEXT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Help for Virtual Assistant 的本地化字符串。
        /// </summary>
        public static string HELP_TITLE {
            get {
                return ResourceManager.GetString("HELP_TITLE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 {
        ///  &quot;$schema&quot;: &quot;http://adaptivecards.io/schemas/adaptive-card.json&quot;,
        ///  &quot;type&quot;: &quot;AdaptiveCard&quot;,
        ///  &quot;version&quot;: &quot;1.0&quot;,
        ///  &quot;speak&quot;: &quot;Welcome to your Virtual Assistant! Now that you&apos;re up and running, let&apos;s get started.&quot;,
        ///  &quot;body&quot;: [
        ///    {
        ///      &quot;type&quot;: &quot;Image&quot;,
        ///      &quot;url&quot;: &quot;intro.JPG&quot;,
        ///      &quot;size&quot;: &quot;stretch&quot;
        ///    },
        ///    {
        ///      &quot;type&quot;: &quot;TextBlock&quot;,
        ///      &quot;spacing&quot;: &quot;medium&quot;,
        ///      &quot;size&quot;: &quot;default&quot;,
        ///      &quot;weight&quot;: &quot;bolder&quot;,
        ///      &quot;text&quot;: &quot;Welcome to **your** Virtual Assistant!&quot;,
        ///      &quot;speak&quot;: &quot;Welcome to your  [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        public static string Intro {
            get {
                return ResourceManager.GetString("Intro", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 .\Dialogs\Main\Resources\Intro.json 的本地化字符串。
        /// </summary>
        public static string INTRO_PATH {
            get {
                return ResourceManager.GetString("INTRO_PATH", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Ok, you&apos;re signed out. 的本地化字符串。
        /// </summary>
        public static string LOGOUT {
            get {
                return ResourceManager.GetString("LOGOUT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Schedule a meeting 的本地化字符串。
        /// </summary>
        public static string MEETING_SUGGESTEDACTION {
            get {
                return ResourceManager.GetString("MEETING_SUGGESTEDACTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 It looks like there is nothing to cancel. What can I help you with? 的本地化字符串。
        /// </summary>
        public static string NO_ACTIVE_DIALOG {
            get {
                return ResourceManager.GetString("NO_ACTIVE_DIALOG", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Find a coffee shop nearby 的本地化字符串。
        /// </summary>
        public static string POI_SUGGESTEDACTION {
            get {
                return ResourceManager.GetString("POI_SUGGESTEDACTION", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 What is Virtual Assistant? 的本地化字符串。
        /// </summary>
        public static string QnA {
            get {
                return ResourceManager.GetString("QnA", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 How is the weather in Shanghai today? 的本地化字符串。
        /// </summary>
        public static string WEATHER {
            get {
                return ResourceManager.GetString("WEATHER", resourceCulture);
            }
        }
    }
}
