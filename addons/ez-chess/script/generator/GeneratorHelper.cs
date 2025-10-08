using System;
using System.Collections.Generic;

public enum ValveTypes
{
  STATE, INSTANCE
}

public record ValveData(string Action, ValveTypes ValveType, bool EventReciveable);

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class RegisterValveAttribute(string action, ValveTypes type, bool eventReciveable = false) : Attribute
{
  public string Action { get; set; } = action;
  public ValveTypes ValveType { get; set; } = type;
  public bool EventReciveable { get; set; } = eventReciveable;
}
public static class AdjectiveConverter
{
    // 辅助方法：判断一个字符是否是元音 (a, e, i, o, u)
    private static bool IsVowel(char c)
    {
        return "aeiou".IndexOf(char.ToLower(c)) >= 0;
    }

    // 辅助方法：判断一个字符是否是辅音
    private static bool IsConsonant(char c)
    {
        return char.IsLetter(c) && !IsVowel(c);
    }

    /// <summary>
    /// 将动词根据英语拼写规则转换为以 "-able" 结尾的形容词，并保留首字母大写。
    /// </summary>
    /// <param name="verb">需要转换的动词。</param>
    /// <returns>转换后的形容词。</returns>
    public static string ToAbleAdjective(string verb)
    {
        if (string.IsNullOrWhiteSpace(verb))
        {
            return verb;
        }

        string trimmedVerb = verb.Trim();
        if (trimmedVerb.Length == 0)
        {
            return string.Empty;
        }

        // 1. 保留原始大小写信息
        bool isFirstCharUpper = char.IsUpper(trimmedVerb[0]);

        // 2. 使用小写动词进行逻辑判断
        string lowerVerb = trimmedVerb.ToLower();
        int len = lowerVerb.Length;

        string resultBase; // 用于存储转换后的小写结果

        // 规则 4: CVC 双写 (简化版)
        if (len >= 3 &&
            IsConsonant(lowerVerb[len - 3]) &&
            IsVowel(lowerVerb[len - 2]) &&
            IsConsonant(lowerVerb[len - 1]) &&
            !"wxy".Contains(lowerVerb[len - 1]))
        {
            var commonCvcVerbs = new HashSet<string> { "stop", "plan", "admit", "forget", "regret", "submit" };
            if (commonCvcVerbs.Contains(lowerVerb))
            {
                resultBase = lowerVerb + lowerVerb[len - 1] + "able";
            }
            else
            {
                // 如果不是已知的CVC词，则按默认规则处理
                resultBase = lowerVerb + "able";
            }
        }
        // 规则 3: 以 "辅音 + y" 结尾
        else if (lowerVerb.EndsWith("y") && len > 1 && IsConsonant(lowerVerb[len - 2]))
        {
            resultBase = lowerVerb.Substring(0, len - 1) + "iable";
        }
        // 规则 2: 以 'e' 结尾
        else if (lowerVerb.EndsWith("e"))
        {
            if (lowerVerb.EndsWith("ce") || lowerVerb.EndsWith("ge"))
            {
                resultBase = lowerVerb + "able";
            }
            else
            {
                resultBase = lowerVerb.Substring(0, len - 1) + "able";
            }
        }
        // 规则 1: 一般情况
        else
        {
            resultBase = lowerVerb + "able";
        }

        // 3. 根据原始大小写格式化最终结果
        if (isFirstCharUpper && resultBase.Length > 0)
        {
            // 将结果的首字母转为大写
            return char.ToUpper(resultBase[0]) + resultBase.Substring(1);
        }
        
        return resultBase;
    }
}
