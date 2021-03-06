﻿using System;
using System.Configuration;
using System.Linq;
using System.Threading;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;

namespace GoogleCalendarAPIv3
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Google Calender API v3");

            var clientId = ConfigurationManager.AppSettings["ClientId"];
            var clientSecret = ConfigurationManager.AppSettings["ClientSecret"];
            var calendarId = ConfigurationManager.AppSettings["CalendarId"];

            try
            {
                var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret,
                    },
                    new[] { CalendarService.Scope.Calendar },
                    "user",
                    CancellationToken.None).Result;

                var service = new CalendarService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Google Calender API v3",
                });

                var queryStart = DateTime.Now;
                var queryEnd = queryStart.AddYears(1);

                var query = service.Events.List(calendarId);
                // query.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime; - not supported :(
                query.TimeMin = queryStart;
                query.TimeMax = queryEnd;

                var events = query.Execute().Items;

                var eventList = events.Select(e => new Tuple<DateTime, string>(DateTime.Parse(e.Start.Date), e.Summary)).ToList();
                eventList.Sort((e1, e2) => e1.Item1.CompareTo(e2.Item1));

                Console.WriteLine("Query from {0} to {1} returned {2} results", queryStart, queryEnd, eventList.Count);

                foreach (var item in eventList)
                {
                    Console.WriteLine("{0}\t{1}", item.Item1, item.Item2);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception encountered: {0}", e.Message);
            }

            Console.WriteLine("Press any key to continue...");

            while (!Console.KeyAvailable)
            {
            }
        }
    }
}
