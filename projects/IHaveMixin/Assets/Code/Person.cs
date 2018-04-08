
/********************************************************************
created:    2018-04-07
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unique
{
    public class Person : IHaveWallet
    {
        public Wallet GetWallet ()
		{
			return _wallet;
		}

		private readonly Wallet _wallet = new Wallet();
    }
}
