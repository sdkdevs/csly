using System;
using System.Linq;
using System.Reflection;

namespace sly.lexer
{
    public class CallBacksBuilder
    {

        public static void BuildCallbacks<IN>(GenericLexer<IN> lexer) where IN : struct
        {
            var attributes =
                (CallBacksAttribute[]) typeof(IN).GetCustomAttributes(typeof(CallBacksAttribute), true);
            Type callbackClass = attributes[0].CallBacksClass;
            ExtractCallBacks(callbackClass,lexer);
            
        }

        public static void ExtractCallBacks<IN>(Type callbackClass, GenericLexer<IN> lexer) where IN : struct
        {
            var methods = callbackClass.GetMethods().ToList();
            methods = methods.Where(m =>
            {
                var attributes = m.GetCustomAttributes().ToList();
                var attr = attributes.Find(a => a.GetType() == typeof(TokenCallbackAttribute));
                return m.IsStatic && attr != null;
            }).ToList();

            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(TokenCallbackAttribute), false).Cast<TokenCallbackAttribute>().ToList<TokenCallbackAttribute>();
                AddCallback(lexer, method, EnumConverter.ConvertIntToEnum<IN>(attributes[0].EnumValue));
            }
        }

        public static void AddCallback<IN>(GenericLexer<IN> lexer, MethodInfo method, IN token) where IN : struct
        {
            var t = typeof(IN);
            var rt = method.ReturnType;
            var ps = method.GetParameters();
            
            var callbackDelegate = (Func<Token<IN>,Token<IN>>)Delegate.CreateDelegate(typeof(Func<Token<IN>,Token<IN>>), method);
           lexer.AddCallBack(token,callbackDelegate);
        }
        
        
    }
}