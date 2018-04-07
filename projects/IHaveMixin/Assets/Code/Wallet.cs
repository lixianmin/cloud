
/********************************************************************
created:    2018-04-07
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unique
{
    public class Wallet
    {
        public void SetMoney (int count)
		{
			_money = count;
		}

		public int GetMoney ()
		{
			return _money;
		}

		private int _money;
    }
}
