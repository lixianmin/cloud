
/********************************************************************
created:    2018-04-07
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unique
{
    public class Game
    {
        public void TestMixin ()
        {
            var person = new Person();
            person.AddMoneyEx(300);
            person.SpendMoneyEx(20);
            var balance = person.GetBalanceEx();
        }
    }
}
