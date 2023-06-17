using Catalog.Domain.Entities;

namespace Catalog.Domain
{
    public static class TicketPriceCalculator
    {
        public static decimal CalculatePrice(decimal basePrice, SectorName sectorName, string city)
        {
            decimal adjustedPrice = basePrice;

            switch (sectorName)
            {
                case SectorName.DanceFloor:
                    adjustedPrice *= 1.8m;
                    break;
                case SectorName.A:
                    adjustedPrice *= 1.7m;
                    break;
                case SectorName.B:
                    adjustedPrice *= 1.6m;
                    break;
                case SectorName.C:
                    adjustedPrice *= 1.5m;
                    break;
                case SectorName.D:
                    adjustedPrice *= 1.4m;
                    break;
                case SectorName.F:
                    adjustedPrice *= 1.3m;
                    break;
                default:
                    adjustedPrice *= 1.3m;
                    break;
            }

            if (city != null)
            {
                switch (city)
                {
                    case "Minsk":
                        adjustedPrice *= 1.5m;
                        break;
                    case "Gomel":
                    case "Grodno":
                    case "Brest":
                    case "Vitebsk":
                        adjustedPrice *= 1.3m;
                        break;
                    default:
                        adjustedPrice *= 1.1m;
                        break;
                }
            }

            return adjustedPrice;
        }
    }
}
