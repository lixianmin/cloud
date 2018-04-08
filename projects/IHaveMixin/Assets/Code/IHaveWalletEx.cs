
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
        public static void AddMoneyEx(this IHaveWallet self, int count)
        {
            if (null != self)
            {
                var wallet = self.GetWallet();
                if (null != wallet)
                {
                    var total = wallet.GetMoney();
                    total += count;
                    wallet.SetMoney(total);
                }
            }
        }

        public static bool SpendMoneyEx(this IHaveWallet self, int count)
        {
            if (null != self && count > 0)
            {
                var wallet = self.GetWallet();
                if (null != wallet)
                {
                    var total = wallet.GetMoney();
                    if (total > count)
                    {
                        total -= count;
                        wallet.SetMoney(total);
                        return true;
                    }
                }
            }

            return false;
        }

        public static int GetBalanceEx(this IHaveWallet self)
        {
            if (null != self)
            {
                var wallet = self.GetWallet();
                if (null != wallet)
                {
                    var total = wallet.GetMoney();
                    return total;
                }
            }

            return 0;
        }
    }
}
