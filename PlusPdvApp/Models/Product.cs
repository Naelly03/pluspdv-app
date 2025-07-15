using Newtonsoft.Json;
using System;
using System.Windows.Resources;


namespace PlusPdvApp.Models;

public class Product
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("is_service")]
    public bool IsService { get; set; }

    [JsonProperty("is_active")]
    public bool IsActive { get; set; }

    [JsonProperty("ean")]
    public string Ean {  get; set; }

    [JsonProperty("reference")]
    public string Reference { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("sales_unit")]
    public string SalesUnit { get; set; }

    [JsonProperty("unit_value")]
    public decimal UnitValue { get; set; }

    [JsonProperty("storage_unit")]
    public string StorageUnit { get; set; }

    [JsonProperty("storage_unit_value")]
    public decimal StorageUnitValue { get; set; }

    [JsonProperty("unit_conversion_factor")]
    public int UnitConversionFactor { get; set; }

    [JsonProperty("weightable")]
    public bool Weightable { get; set; }

    [JsonProperty("unit_cost_value")]
    public decimal UnitCostValue { get; set; }

    [JsonProperty("unit_purchase_value")]
    public decimal UnitPurchaseValue { get; set; }

    [JsonProperty("net_profit_percentage")]
    public decimal NetProfitPercentage { get; set; }

    [JsonProperty("has_image")]
    public bool HasImage { get; set; }

    [JsonProperty("last_image_update")]
    public DateTime LastImageUpdate { get; set; }

    [JsonProperty("discount_rate_limit")]
    public decimal DiscountRateLimit { get; set; }

    [JsonProperty("discount_rate_locked")]
    public bool DiscountRateLocked { get; set; }
    
    [JsonProperty("section")]
    public Section Section { get; set; } 

    [JsonProperty("subsection")]
    public Subsection Subsection { get; set; }

    [JsonProperty("supplier")]
    public SupplierManufacturer Supplier { get; set; } 

    [JsonProperty("manufacturer")]
    public SupplierManufacturer Manufacturer { get; set; } 

    [JsonProperty("remarks")]
    public string Remarks { get; set; }

    [JsonProperty("last_update")]
    public DateTime LastUpdate { get; set; }
}
