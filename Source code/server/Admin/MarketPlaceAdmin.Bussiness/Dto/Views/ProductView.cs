﻿using MarketPlace.DataAccess.Model;

namespace MarketPlaceAdmin.Bussiness.Dto.Views
{
    public class ProductView
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string? Thumbnail { get; set; }

        public string? ProductDescription { get; set; }

        public string? Location { get; set; }

        public double Price { get; set; }

        public DateTime CreatedDate { get; set; }

        public byte Status { get; set; }

        public ProductView(Product product, string? thumbnail)
        {
            ProductId = product.ProductId;
            ProductName = product.ProductName;
            CategoryId = product.CategoryId;
            CategoryName = product.Category.CategoryName;
            Thumbnail = thumbnail;
            ProductDescription = product.ProductDescription;
            Location = product.Address;
            Price = product.Price;
            CreatedDate = product.CreatedDate;
            Status = (byte)product.Status;
        }
    }
}
