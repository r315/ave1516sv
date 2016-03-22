﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jsonzai.Test.Model
{
    public class Student
    {
        public int[] test = { 1, 2, 3, 4 };
        public int nr;
        public string name;
        

        public int [] Test{
            get { return test; }
            set { test = value; }
            }

        public Student(int nr, string name)
        {
            this.nr = nr;
            this.name = name;
        }   

        public int Nr {
            get{
                return nr;
            }
            set {
                nr = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Student other = obj as Student;
            if (other == null) return false;
            return nr == other.nr && name.Equals(other.name);
        }
    }

}
