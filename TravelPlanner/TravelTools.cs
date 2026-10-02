using System;
using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace TravelPlanner
{
    /// <summary>
    /// Travel Planner tools.
    ///
    /// Every method marked with [KernelFunction]
    /// can be called automatically by the AI model.
    ///
    /// The Description attributes tell the model
    /// when a tool should be used.
    /// </summary>
    public class TravelTools
    {
        [KernelFunction("get_current_date")]
        [Description(
            "Gets today's current date. Use this when the user asks for planning based on today's date or relative dates such as next week, next month, or upcoming weekends."
        )]
        public string GetCurrentDate()
        {
            Console.WriteLine("   [Tool Called] get_current_date()");

            return DateTime.Today.ToString("yyyy-MM-dd");
        }

        [KernelFunction("calculate_trip_days")]
        [Description(
            "Calculates the total number of travel days between a start date and an end date. Always use this before creating an itinerary when the user provides travel dates."
        )]
        public string CalculateTripDays(
            [Description("Trip start date in yyyy-MM-dd format")]
            string startDate,

            [Description("Trip end date in yyyy-MM-dd format")]
            string endDate)
        {
            Console.WriteLine(
                $"   [Tool Called] calculate_trip_days({startDate}, {endDate})");

            if (!DateTime.TryParse(startDate, out DateTime start))
            {
                return "Invalid start date.";
            }

            if (!DateTime.TryParse(endDate, out DateTime end))
            {
                return "Invalid end date.";
            }

            int days = (end.Date - start.Date).Days + 1;

            if (days <= 0)
            {
                return "End date must be after start date.";
            }

            return days.ToString();
        }

        [KernelFunction("get_weather")]
        [Description(
            "Gets expected weather information for a travel destination. Use this when creating travel plans, destination suggestions, packing recommendations, or answering weather-related travel questions."
        )]
        public string GetWeather(
            [Description("Destination city or state")]
            string destination)
        {
            Console.WriteLine(
                $"   [Tool Called] get_weather({destination})");

            string city = destination.Trim().ToLower();

            if (city.Contains("goa"))        return "Sunny and humid, around 31°C.";
            if (city.Contains("jaipur"))     return "Hot and dry, around 34°C.";
            if (city.Contains("kerala"))     return "Warm with occasional rain, around 29°C.";
            if (city.Contains("manali"))     return "Cool and pleasant, around 18°C.";
            if (city.Contains("shimla"))     return "Cool weather, around 16°C.";
            if (city.Contains("mumbai") || city.Contains("bombay"))
                                             return "Humid and warm, around 30°C.";
            if (city.Contains("delhi"))      return "Warm with cool winters, around 28°C.";
            if (city.Contains("agra"))       return "Hot and dry, around 33°C.";
            if (city.Contains("ladakh") || city.Contains("leh"))
                                             return "Cold and dry, around 5°C. Expect sub-zero nights.";
            if (city.Contains("varanasi"))   return "Hot and humid, around 32°C.";
            if (city.Contains("rishikesh"))  return "Pleasant with cool breeze, around 24°C.";
            if (city.Contains("darjeeling")) return "Cool and misty, around 12°C.";
            if (city.Contains("coorg"))      return "Cool and rainy, around 20°C.";

            return "Pleasant weather, around 25°C.";
        }

        [KernelFunction("calculate_budget")]
        [Description(
            "Calculates an estimated travel budget based on destination and total travel days. Use when the user asks for budget trips, travel costs, expense estimation, or itinerary planning."
        )]
        public string CalculateBudget(
            [Description("Destination city")]
            string destination,

            // NOTE: string type used because the model passes the output of
            // calculate_trip_days (which returns a string) directly here.
            [Description("Total trip days as a plain number, e.g. 5")]
            string days)
        {
            Console.WriteLine(
                $"   [Tool Called] calculate_budget({destination}, {days})");

            if (!int.TryParse(days, out int numDays) || numDays <= 0)
            {
                return "Invalid number of days provided.";
            }

            int costPerDay = 3000;
            string city = destination.Trim().ToLower();

            if (city.Contains("goa"))        costPerDay = 3500;
            else if (city.Contains("manali"))     costPerDay = 3200;
            else if (city.Contains("jaipur"))     costPerDay = 2800;
            else if (city.Contains("mumbai") || city.Contains("bombay"))
                                                  costPerDay = 4000;
            else if (city.Contains("delhi"))      costPerDay = 3000;
            else if (city.Contains("agra"))       costPerDay = 2500;
            else if (city.Contains("ladakh") || city.Contains("leh"))
                                                  costPerDay = 4500;
            else if (city.Contains("varanasi"))   costPerDay = 2500;
            else if (city.Contains("rishikesh"))  costPerDay = 2000;
            else if (city.Contains("darjeeling")) costPerDay = 2800;
            else if (city.Contains("coorg"))      costPerDay = 3000;

            int total = costPerDay * numDays;

            return $"Estimated budget for {numDays} day(s) in {destination} is approximately \u20b9{total}.";
        }

        [KernelFunction("get_attractions")]
        [Description(
            "Returns famous tourist attractions and places to visit in a destination. Use when creating itineraries, sightseeing plans, or destination recommendations."
        )]
        public string GetAttractions(
            [Description("Destination city or state")]
            string destination)
        {
            Console.WriteLine(
                $"   [Tool Called] get_attractions({destination})");

            string city = destination.Trim().ToLower();

            if (city.Contains("goa"))
                return "Baga Beach, Calangute Beach, Fort Aguada, Anjuna Beach, Dudhsagar Falls";

            if (city.Contains("jaipur"))
                return "Amber Fort, City Palace, Hawa Mahal, Nahargarh Fort, Jantar Mantar";

            if (city.Contains("kerala"))
                return "Munnar, Alleppey Backwaters, Thekkady, Wayanad, Kovalam Beach";

            if (city.Contains("manali"))
                return "Solang Valley, Rohtang Pass, Hidimba Temple, Mall Road, Beas River";

            if (city.Contains("shimla"))
                return "The Ridge, Mall Road, Jakhu Temple, Kufri, Christ Church";

            if (city.Contains("mumbai") || city.Contains("bombay"))
                return "Gateway of India, Marine Drive, Elephanta Caves, Juhu Beach, Siddhivinayak Temple";

            if (city.Contains("delhi"))
                return "Red Fort, Qutub Minar, India Gate, Humayun's Tomb, Lotus Temple";

            if (city.Contains("agra"))
                return "Taj Mahal, Agra Fort, Fatehpur Sikri, Mehtab Bagh";

            if (city.Contains("ladakh") || city.Contains("leh"))
                return "Pangong Lake, Nubra Valley, Leh Palace, Magnetic Hill, Khardung La Pass";

            if (city.Contains("varanasi"))
                return "Dashashwamedh Ghat, Kashi Vishwanath Temple, Sarnath, Manikarnika Ghat";

            if (city.Contains("rishikesh"))
                return "Laxman Jhula, Ram Jhula, Triveni Ghat, Beatles Ashram, River Rafting";

            if (city.Contains("darjeeling"))
                return "Tiger Hill, Batasia Loop, Tea Gardens, Darjeeling Himalayan Railway, Zoo";

            if (city.Contains("coorg"))
                return "Abbey Falls, Raja's Seat, Nagarhole National Park, Talacauvery, Coffee Plantations";

            return "Popular local sightseeing spots and cultural attractions.";
        }

        [KernelFunction("get_best_season")]
        [Description(
            "Returns the best season or months to visit a destination. Use when the user asks for the ideal time to travel."
        )]
        public string GetBestSeason(
            [Description("Destination city or state")]
            string destination)
        {
            Console.WriteLine(
                $"   [Tool Called] get_best_season({destination})");

            string city = destination.Trim().ToLower();

            if (city.Contains("goa"))        return "November to February";
            if (city.Contains("jaipur"))     return "October to March";
            if (city.Contains("kerala"))     return "September to March";
            if (city.Contains("manali"))     return "March to June";
            if (city.Contains("shimla"))     return "March to June and September to November";
            if (city.Contains("mumbai") || city.Contains("bombay"))
                                             return "November to February";
            if (city.Contains("delhi"))      return "October to March";
            if (city.Contains("agra"))       return "October to March";
            if (city.Contains("ladakh") || city.Contains("leh"))
                                             return "June to September";
            if (city.Contains("varanasi"))   return "October to March";
            if (city.Contains("rishikesh"))  return "September to June";
            if (city.Contains("darjeeling")) return "March to May and September to November";
            if (city.Contains("coorg"))      return "October to March";

            return "Most months are suitable for travel.";
        }

        [KernelFunction("get_packing_list")]
        [Description(
            "Provides travel packing recommendations based on destination weather and travel conditions. Use when creating a complete travel plan."
        )]
        public string GetPackingList(
            [Description("Destination city or state")]
            string destination)
        {
            Console.WriteLine(
                $"   [Tool Called] get_packing_list({destination})");

            string city = destination.Trim().ToLower();

            if (city.Contains("goa"))
                return "Sunscreen, Sunglasses, Beachwear, Hat, Light Clothing, Flip Flops";

            if (city.Contains("manali") || city.Contains("shimla"))
                return "Jacket, Thermals, Gloves, Woollen Socks, Trekking Shoes, Lip Balm";

            if (city.Contains("kerala"))
                return "Umbrella, Light Clothing, Comfortable Footwear, Insect Repellent";

            if (city.Contains("mumbai") || city.Contains("bombay"))
                return "Light Clothing, Umbrella (monsoon), Comfortable Walking Shoes";

            if (city.Contains("delhi") || city.Contains("agra") || city.Contains("jaipur"))
                return "Comfortable Clothing, Cap, Sunscreen, Walking Shoes, Scarf";

            if (city.Contains("ladakh") || city.Contains("leh"))
                return "Heavy Jacket, Thermals, Gloves, Warm Socks, Sunglasses, Altitude Sickness Medicine";

            if (city.Contains("varanasi"))
                return "Light Cotton Clothing, Comfortable Walking Shoes, Scarf, Sunscreen";

            if (city.Contains("rishikesh"))
                return "Sports Clothes, Trekking Shoes, Waterproof Bag, Sunscreen, Light Jacket";

            if (city.Contains("darjeeling"))
                return "Warm Jacket, Thermals, Woollen Cap, Gloves, Trekking Shoes";

            if (city.Contains("coorg"))
                return "Light Rain Jacket, Trekking Shoes, Insect Repellent, Light Clothing";

            return "Comfortable Clothing, Power Bank, Personal Medicines, Identity Documents";
        }
    }
}