using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace mRides_app
{
    public class TestMe
    {

        private int ID;
        private int counter;


        public static int OneToFive()
        {
            Random rand = new Random();
            int x = rand.Next(1, 5);
            return x;
        }


        public TestMe(int ID)
        {
            this.ID = ID;
            this.counter = 0;
        }

        public void incrementCounter()
        {
            this.counter++;
        }

        public int getCounter()
        {
            return this.counter;
        }
        
        
    }
}