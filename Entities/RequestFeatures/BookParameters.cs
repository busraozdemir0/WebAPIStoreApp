﻿namespace Entities.RequestFeatures
{
    public class BookParameters:RequestParameters
	{
		// Fiyat filtrelemesi için
		public uint MinPrice { get; set; } // uint => Price ifadesi negatif olamayacağı için
		public uint MaxPrice { get; set; } = 1000;
		public bool ValidPriceRange => MaxPrice > MinPrice;
		public String? SearchTerm { get; set; }

		public BookParameters()
		{
			OrderBy = "id"; // herhangi bri şey belirtilmediyse id'ye göre sıralama yapılsın.
		}
	}
}
