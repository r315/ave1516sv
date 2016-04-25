using System;
using System.Reflection;
using Logging;

public class Info {

    [DontLog]
    public int a;

    public int b;

    [LogLevel(2)]
    public int c;

    [LogLevel(4)]
    public int d;
    
    public Info(int aa, int bb, int cc, int dd) 
    {
        a = aa; b = bb; c = cc; d = dd;
    }
}

public class User {

    public int userId;
    
    public string username;

    [DontLog]
    public string password;

    [LogLevel(3)]
    public string fullName;

    [LogLevel(5)]
    public double factor;
    
    public User(int userId, string username, string password, string fullName, double factor) 
    {
        this.userId   = userId;
        this.username = username;
        this.password = password;
        this.fullName = fullName;
        this.factor   = factor;
    }
}

public class Test
{
    public static void Main()
    {
        Info info = new Info(1, 2, 3, 4);
        User user = new User(1, "afonso1", "elReX", "Afonso Henriques", 1.95);
        Info inf2 = new Info(5, 6, 7, 8);
        
        Logger.Log(info, 2);
        Logger.Log(user, 4);
        Logger.Log(user, 2);
        Logger.Log(inf2, 7);
    }
}
