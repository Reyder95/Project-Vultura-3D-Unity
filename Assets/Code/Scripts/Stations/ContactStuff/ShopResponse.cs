using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopResponse : BaseResponse
{
    private Market market;

    public ShopResponse(string prompt, Market market) : base(prompt, VulturaInstance.ResponseType.Shop)
    {
        this.market = market;
    }

    public Market Market
    {
        get
        {
            return this.market;
        }
    }
}
