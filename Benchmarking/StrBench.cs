using System;
using System.Text;

public class StrBench
{
    private static readonly String[] GREEK_CHARS = new String[]
    {
        "Alpha", "Beta", "Gamma", "Delta", "Epsilon",
        "Zeta", "Eta", "Theta", "Iota", "Kappa", "Lambda",
        "Mu", "Nu", "Xi", "Omicron", "Pi", "Rho", "Sigma",
        "Tau", "Upsilon", "Phi", "Chi", "Psi", "Omega"
    };

    public static StringBuilder strForBuilder;
    public static String        strForPlus;

    private static String JoinWithJoin(String[] words)
    {
        return String.Join("", words);
    }

    private static String JoinWithBuilder(String[] words)
    {
        strForBuilder = new StringBuilder();
        for (int i = 0; i < words.Length; ++i) {
            strForBuilder.Append(words[i]);
        }
        return strForBuilder.ToString();
    }

    private static String JoinWithPlus(String[] words)
    {
        strForPlus = "";
        for (int i = 0; i < words.Length; ++i) {
            strForPlus += words[i];
        }
        return strForPlus;
    }

    public static Object callJoinWithJoin()
    {
        return JoinWithJoin(GREEK_CHARS);
    }

    public static Object callJoinWithBuilder()
    {
        return JoinWithBuilder(GREEK_CHARS);
    }

    public static Object callJoinWithPlus()
    {
        return JoinWithPlus(GREEK_CHARS);
    }
    
    public static void Main()
    {
        NBench.Benchmark(() => null, "NoJoin");
        NBench.Benchmark(callJoinWithJoin, "JoinWithJoin");
        NBench.Benchmark(callJoinWithBuilder, "JoinWithBuilder");
        NBench.Benchmark(callJoinWithPlus, "JoinWithPlus");
    }
}
