using System;
using System.Collections.Generic;

public class Order
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public List<OrderItem> Items { get; set; }
}