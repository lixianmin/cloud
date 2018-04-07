
/********************************************************************
created:    2018-04-07
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unique
{
    public static class IHaveWalletEx
    {
        public static void AddMoneyEx (this Wallet wallet, int count)
        {
            if (null != wallet)
            {
                var total = wallet.GetMoney();
                total += count;
                wallet.SetMoney(total);
            }
        }
    }
}
