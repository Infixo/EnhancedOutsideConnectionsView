﻿using UnityEngine;
using System.Reflection;

namespace EnhancedOutsideConnectionsView
{
    /// <summary>
    /// Harmony patching for building AI
    /// </summary>
    public class BuildingAIPatch
    {
        // DLC = Down Loadable Content
        // CCP = Content Creator Pack

        // If a building is not available because a DLC/CCP is not installed, the building AI still remains in the game logic.
        // The corresponding building AI patch will simply never be called because there will be no buildings of that type.
        // Therefore, there is no need to avoid patching a building AI for missing DLC/CCP.

        // buildings introduced in DLC:
        // BG = Base Game               03/10/15
        // AD = After Dark              09/24/15 AfterDarkDLC
        // SF = Snowfall                02/18/16 SnowFallDLC
        // MD = Match Day               06/09/16 Football
        // ND = Natural Disasters       11/29/16 NaturalDisastersDLC
        // MT = Mass Transit            05/18/17 InMotionDLC
        // CO = Concerts                08/17/17 MusicFestival
        // GC = Green Cities            10/19/17 GreenCitiesDLC
        // PL = Park Life               05/24/18 ParksDLC
        // IN = Industries              10/23/18 IndustryDLC
        // CA = Campus                  05/21/19 CampusDLC
        // SH = Sunset Harbor           03/26/20 UrbanDLC
        // AP = Airports                01/25/22 AirportDLC

        // buildings introduced in CCP:
        // DE = Deluxe Edition          03/10/15 DeluxeDLC
        // AR = Art Deco                09/01/16 ModderPack1
        // HT = High Tech Buildings     11/29/16 ModderPack2
        // PE = Pearls From the East    03/22/17 OrientalBuildings
        // ES = European Suburbia       10/19/17 ModderPack3 - no unique buildings, new style, "80 new special residential buildings and props"
        // UC = University City         05/21/19 ModderPack4 - no unique buildings, "adds 36 low-density residential buildings, 32 low-density commercial buildings, and 15 props"
        // MC = Modern City Center      11/07/19 ModderPack5 - no unique buildings, new style, "adds 39 unique models featuring new modern commercial wall-to-wall buildings"
        // MJ = Modern Japan            03/26/20 ModderPack6
        // TS = Train Stations          05/21/21 ModderPack7
        // BP = Bridges & Piers         05/21/21 ModderPack8
        // MP = Map Pack                01/25/22 ModderPack9 - no unique buildings, "8 new maps"
        // VW = Vehicles of the World   01/25/22 ModderPack10 - no unique buildings, "set of 21 new vehicle assets"



        // For building AIs that derive from other building AIs (i.e. not derived from PrivateBuildingAI or PlayerBuildingAI):
        //     If the derived building AI has its own GetColor method with logic for Outside Connections, it is patched.
        //     If the derived building AI has no GetColor method, Harmony won't allow it to be patched.
        //         But the base building AI with logic for Outside Connections is patched and that patch will handle the derived building AI.
        // Each building AI that has its own GetColor method is marked with GC below.
        // Each building AI where its GetColor method has logic for Outside Connections is additionally marked with OC below.
        // Note that the building AIs that derive directly from PrivateBuildingAI or PlayerBuildingAI all have a GetColor method.


        // zoned building AIs are derived from PrivateBuildingAI

        // ResidentialBuildingAI        GC      Zoned Generic Low Density BG, Zoned Generic High Density BG, Zoned Specialized Residential (Self-Sufficient Buildings GC)
        // CommercialBuildingAI         GC  OC  Zoned Generic Low Density BG, Zoned Generic High Density BG, Zoned Specialized Commercial (Tourism AD, Leisure AD, Organic and Local Produce GC)
        // OfficeBuildingAI             GC  OC  Zoned Generic Office BG, Zoned Specialized Office (IT Cluster GC)
        // IndustrialBuildingAI         GC  OC  Zoned Generic Industrial BG
        // IndustrialExtractorAI        GC  OC  Zoned Specialized Industrial (Forest BG, Farming BG, Ore BG, Oil BG)
        //    LivestockExtractorAI              Zoned Specialized Industrial (Farming BG)

        // the following building AIs are from the Ploppable RICO Revisited mod
        // the growable  building AIs derive from the above zoned building AIs
        // the ploppable building AIs derive from the growable building AIs
        // none of the growable/ploppable building AIs have a GetColor method
        // PloppableRICO.GrowableResidentialAI  PloppableRICO.PloppableResidentialAI
        // PloppableRICO.GrowableCommercialAI   PloppableRICO.PloppableCommercialAI
        // PloppableRICO.GrowableOfficeAI       PloppableRICO.PloppableOfficeAI
        // PloppableRICO.GrowableIndustrialAI   PloppableRICO.PloppableIndustrialAI
        // PloppableRICO.GrowableExtractorAI    PloppableRICO.PloppableExtractorAI
        // the Ploppable RICO Revisited mod does not have building AIs corresponding to LivestockExtractorAI


        // service building AIs are derived from PlayerBuildingAI

        // AirportBuildingAI            GC      (base class with no buildings) AP
        //    AirportAuxBuildingAI              Control Tower AP (3 styles), Concourse Hub AP (3 styles), Small Hangar AP, Large Hangar AP,
        //                                      Budget Airport Hotel AP, Luxury Airport Hotel AP, Airline Lounge AP, Aviation Fuel Station AP,
        //                                      Small Parked Plane AP (3 variations), Medium Parked Plane AP (3 variations), Large Parked Plane AP (2 variations), Parked Cargo Plane AP (2 variations)
        //    AirportEntranceAI                 Airport Terminal AP (3 styles), Two-Story Terminal AP (3 styles), Large Terminal AP (3 styles), Cargo Airport Terminal AP
        // CargoStationAI               GC      Cargo Train Terminal BG, Cargo Airport IN, Cargo Airport Hub IN, Airport Cargo Train Station AP
        //    AirportCargoGateAI        GC      Cargo Aircraft Stand AP
        //    CargoHarborAI                     Cargo Harbor BG, Cargo Hub AD
        // CemeteryAI                   GC      Cemetery BG, Crematorium BG, Cryopreservatory HT (CCP)
        // ChildcareAI                  GC      Child Health Center BG
        // DepotAI                      GC      Taxi Depot AD (vehicle count can be deteremined, but Taxi Depot is treated like it has unlimited)
        // DepotAI                      GC      Bus Depot BG, Biofuel Bus Depot GC, Trolleybus Depot SH, Tram Depot SF, Ferry Depot MT, Helicopter Depot SH, Blimp Depot MT, Sightseeing Bus Depot PL
        //    CableCarStationAI                 Cable Car Stop MT, End-of-Line Cable Car Stop MT
        //    TransportStationAI                Bus Station AD, Helicopter Stop SH, Blimp Stop MT
        //    TransportStationAI                Intercity Bus Station SH, Intercity Bus Terminal SH,
        //                                      Metro Station BG, Elevated Metro Station BG, Underground Metro Station BG, Metro Plaza Station TS (aka H_Hub02_A),
        //                                      Sunken Island Platform Metro Station TS, Sunken Dual Island Platform Metro Station TS, Sunken Bypass Metro Station TS,
        //                                      Elevated Island Platform Metro Station TS, Elevated Dual Island Platform Metro Station TS, Elevated Bypass Metro Station TS,
        //                                      Train Station BG, Crossover Train Station Hub TS (aka H_Hub03), Old Market Station TS (aka H_Hub04),
        //                                      Ground Island Platform Train Station TS, Ground Dual Island Platform Train Station TS, Ground Bypass Train Station TS,
        //                                      Elevated Island Platform Train Station TS, Elevated Dual Island Platform Train Station TS, Elevated Bypass Train Station TS,
        //                                      Airport BG, Monorail Station MT, Monorail Station with Road MT,
        //                                      Bus-Intercity Bus Hub SH (aka Transport Hub 02 A), Bus-Metro Hub SH (aka Transport Hub 05 A), Metro-Intercity Bus Hub SH (aka Transport Hub 01 A),
        //                                      Train-Metro Hub SH (aka Transport Hub 03 A), Glass Box Transport Hub TS (aka H_Hub01), Multiplatform End Station MT, Multiplatform Train Station MT,
        //                                      International Airport AD, Metropolitan Airport SH (aka Transport Hub 04 A),
        //                                      Monorail-Bus Hub MT, Metro-Monorail-Train Hub MT
        //       AirportGateAI          GC      Airport Bus Station AP, Small Aircraft Stand AP, Medium Aircraft Stand AP, Large Aircraft Stand AP, Elevated Airport Metro Station AP, Airport Train Station AP
        //       HarborAI                       Ferry Stop MT, Ferry Pier MT, Ferry and Bus Exchange Stop MT
        //       HarborAI                       Harbor BG
        // DisasterResponseBuildingAI   GC      Disaster Response Unit ND
        // DoomsdayVaultAI              GC      Doomsday Vault ND (monument)
        // EarthquakeSensorAI           GC      Earthquake Sensor ND
        // EldercareAI                  GC      Eldercare BG
        // FireStationAI                GC      Fire House BG, Fire Station BG
        // FirewatchTowerAI             GC      Firewatch Tower ND
        // FishFarmAI                   GC  OC  Fish Farm SH, Algae Farm SH, Seaweed Farm SH
        // FishingHarborAI              GC  OC  Fishing Harbor SH, Anchovy Fishing Harbor SH, Salmon Fishing Harbor SH, Shellfish Fishing Harbor SH, Tuna Fishing Harbor SH
        // HadronColliderAI             GC      Hadron Collider BG (monument)
        // HeatingPlantAI               GC  OC  Boiler Station SF, Geothermal Heating Plant SF
        // HelicopterDepotAI            GC      Medical Helicopter Depot ND, Fire Helicopter Depot ND, Police Helicopter Depot ND
        // HospitalAI                   GC      Medical Laboratory HT (CCP)
        // HospitalAI                   GC      Medical Clinic BG, Hospital BG, General Hospital SH (CCP)
        //    MedicalCenterAI                   Medical Center BG (monument)
        // IndustryBuildingAI           GC      (base clase with no buildings)
        //    AuxiliaryBuildingAI       GC      Forestry:  IN: Forestry Workers’ Barracks, Forestry Maintenance Building
        //                                      Farming:   IN: Farm Workers’ Barracks, Farm Maintenance Building
        //                                      Ore:       IN: Ore Industry Workers’ Barracks, Ore Industry Maintenance Building
        //                                      Oil:       IN: Oil Industry Workers’ Barracks, Oil Industry Maintenance Building
        //    ExtractingFacilityAI      GC  OC  Forestry:  IN: Small Tree Plantation, Medium Tree Plantation, Large Tree Plantation, Small Tree Sapling Greenhouse, Large Tree Sapling Greenhouse
        //                                      Farming:   IN: Small Crops Greenhouse, Medium Crops Greenhouse, Large Crops Greenhouse, Small Fruit Greenhouse, Medium Fruit Greenhouse, Large Fruit Greenhouse
        //                                      Ore:       IN: Small Ore Mine, Medium Ore Mine, Large Ore Mine, Small Ore Mine Underground, Large Ore Mine Underground, Seabed Mining Vessel
        //                                      Oil:       IN: Small Oil Pump, Large Oil Pump, Small Oil Drilling Rig, Large Oil Drilling Rig, Offshore Oil Drilling Platform
        //    ProcessingFacilityAI      GC  OC  Forestry:  IN: Sawmill, Biomass Pellet Plant, Engineered Wood Plant, Pulp Mill
        //                                      Farming:   IN: Small Animal Pasture, Large Animal Pasture, Flour Mill, Cattle Shed, Milking Parlor,
        //                                      Ore:       IN: Ore Grinding Mill, Glass Manufacturing Plant, Rotary Kiln Plant, Fiberglass Plant
        //                                      Oil:       IN: Oil Sludge Pyrolysis Plant, Petrochemical Plant, Waste Oil Refining Plant, Naphtha Cracker Plant
        //                                      Fishing:   SH: Fish Factory
        //       UniqueFactoryAI                IN: Furniture Factory, Bakery, Industrial Steel Plant, Household Plastic Factory, Toy Factory, Printing Press, Lemonade Factory, Electronics Factory,
        //                                          Clothing Factory, Petroleum Refinery, Soft Paper Factory, Car Factory, Food Factory, Sneaker Factory, Modular House Factory, Shipyard
        // LandfillSiteAI               GC  xx  Landfill Site BG, Incineration Plant BG, Recycling Center GC, Waste Transfer Facility SH, Waste Processing Complex SH, Waste Disposal Unit SH (CCP)
        //                                      Has Outside Connections logic, but the logic always returns neutral color, so don't include this AI.
        //    UltimateRecyclingPlantAI          Ultimate Recycling Plant GC (monument)
        // LibraryAI                    GC      Public Library BG
        // MainCampusBuildingAI         GC      Trade School Administration Building CA, Liberal Arts Administration Building CA, University Administration Building CA
        // MainIndustryBuildingAI       GC      Forestry Main Building IN, Farm Main Building IN, Ore Industry Main Building IN, Oil Industry Main Building IN
        // MaintenanceDepotAI           GC      Road Maintenance Depot SF, Park Maintenance Building PL
        // MarketAI                     GC  OC  Fish Market SH
        // MonumentAI                   GC      Landmarks:          ChirpX Launch Site BG
        // MonumentAI                   GC      Landmarks:          Hypermarket BG, Government Offices BG, The Gherkin BG, London Eye BG, Sports Arena BG, Theatre BG, Shopping Center BG,
        //                                                          Cathedral BG, Amsterdam Palace BG, Winter Market BG, Department Store BG, City Hall BG, Cinema BG,
        //                                                          Panda Sanctuary PE, Oriental Pearl Tower PE, Temple Complex PE,
        //                                                          Traffic Park MT, Boat Museum MT, Locomotive Halls MT
        //                                      Deluxe Edition:     Statue of Liberty DE, Eiffel Tower DE, Grand Central Terminal DE, Arc de Triomphe DE, Brandenburg Gate DE
        //                                      Tourism & Leisure:  Icefishing Pond AD+SF, Casino AD, Driving Range AD, Fantastic Fountain AD, Frozen Fountain AD+SF, Luxury Hotel AD, Zoo AD
        //                                      Winter Unique:      Ice Hockey Arena SF, Ski Resort SF, Snowcastle Restaurant SF, Spa Hotel SF, Sleigh Ride SF, Snowboard Arena SF, The Christmas Tree SF, Igloo Hotel SF
        //                                      Match Day:          Football Stadium MD
        //                                      Concerts:           Festival Area CO, Media Broadcast Building CO, Music Club CO, Fan Zone Park CO
        //                                      Airports:           Aviation Museum AP
        //                                      Level 1 Unique:     Statue of Industry BG, Statue of Wealth BG, Lazaret Plaza BG, Statue of Shopping BG, Plaza of the Dead BG,
        //                                                          Meteorite Park ND, Bird and Bee Haven GC, City Arch PL
        //                                      Level 2 Unique:     Fountain of Life and Death BG, Friendly Neighborhood Park BG, Transport Tower BG, Mall of Moderation BG, Posh Mall BG,
        //                                                          Disaster Memorial ND, Climate Research Station GC, Clock Tower PL
        //                                      Level 3 Unique:     Colossal Order Offices BG, Official Park BG, Court House BG, Grand Mall BG, Tax Office BG,
        //                                                          Helicopter Park ND, Lungs of the City GC, Old Market Street PL
        //                                      Level 4 Unique:     Business Park BG, Grand Library BG, Observatory BG, Opera House BG, Oppression Office BG,
        //                                                          Pyramid Of Safety ND, Floating Gardens GC, Sea Fortress PL
        //                                      Level 5 Unique:     Servicing Services Offices BG, Academic Library BG, Science Center BG, Expo Center BG, High Interest Tower BG, Aquarium BG,
        //                                                          Sphinx Of Scenarios ND, Ziggurat Garden GC, Observation Tower PL
        //                                      Level 6 Unique:     Cathedral of Plenitude BG, Stadium BG, MAM Modern Art Museum BG, Sea-and-Sky Scraper BG, Theater of Wonders BG,
        //                                                          Sparkly Unicorn Rainbow Park ND, Central Park GC, The Statue of Colossalus PL
        //                                      Content Creator:    Eddie Kovanago AR, Pinoa Street AR, The Majesty AR, Electric Car Factory HT, Nanotechnology Center HT, Research Center HT,
        //                                                          Robotics Institute HT, Semiconductor Plant HT, Software Development Studio HT, Space Shuttle Launch Site HT, Television Station HT,
        //                                                          Drive-in Restaurant MJ, Drive-in Oriental Restaurant MJ, Oriental Market MJ, Noodle Restaurant MJ, Ramen Restaurant MJ,
        //                                                          Service Station and Restaurant MJ, Small Office Building MJ, City Office Building MJ, District Office Building MJ,
        //                                                          Local Register Office MJ, Resort Hotel MJ, Downtown Hotel MJ, Temple MJ, High-rise Office Building MJ,
        //                                                          Company Headquarters MJ, Office Skyscraper MJ, The Station Department Store MJ, The Rail Yard Shopping Center MJ
        //    AirlineHeadquartersAI             Airline Headquarters Building AP
        //    AnimalMonumentAI                  Winter Unique:   Santa Claus' Workshop SF
        //    ChirpwickCastleAI                 Castle Of Lord Chirpwick PL (monument)
        //    MuseumAI                          The Technology Museum CA, The Art Gallery CA, The Science Center CA
        //    PrivateAirportAI                  Aviation Club SH (Level 5 Unique)
        //    VarsitySportsArenaAI      GC      Aquatics Center CA, Basketball Arena CA, Track And Field Stadium CA, Baseball Park CA, American Football Stadium CA
        // ParkAI                       GC      Parks:  Small Park BG, Small Playground BG, Park With Trees BG, Large Playground BG, Bouncy Castle Park BG, Botanical Garden BG,
        //                                              Dog Park BG, Carousel Park BG, Japanese Garden BG, Tropical Garden BG, Fishing Island BG, Floating Cafe BG,
        //                                              Snowmobile Track AD+SF, Winter Fishing Pier AD+SF, Ice Hockey Rink AD+SF
        //                                      Plazas:             Plaza with Trees BG, Plaza with Picnic Tables BG, Paradox Plaza BG (special)
        //                                      Other Parks:        Basketball Court BG, Tennis Court BG
        //                                      Tourism & Leisure:  Fishing Pier AD, Fishing Tours AD, Jet Ski Rental AD, Marina AD, Restaurant Pier AD, Beach Volleyball Court AD, Riding Stable AD, Skatepark AD
        //                                      Winter Parks:       Snowman Park SF, Ice Sculpture Park SF, Sledding Hill SF, Curling Park SF, Skating Rink SF, Ski Lodge SF, Cross-Country Skiing Park SF, Firepit Park SF
        //                                      Content Creator:    Seine Pier BP, Rhine Pier BP, Biodome HT, Vertical Farm HT
        //    EdenProjectAI                     Eden Project BG (monument)
        // ParkBuildingAI               GC      Only Amusement Park and Zoo have workers.
        //                                      City Park:       PL: Park Plaza, Park Cafe #1, Park Restrooms #1, Park Info Booth #1, Park Chess Board #1, Park Pier #1, Park Pier #2
        //                                      Amusement Park:  PL: Amusement Park Plaza, Amusement Park Cafe #1, Amusement Park Souvenir Shop #1, Amusement Park Restrooms #1, Game Booth #1, Game Booth #2,
        //                                                           Carousel, Piggy Train, Rotating Tea Cups, Swinging Boat, House Of Horrors, Bumper Cars, Drop Tower Ride, Pendulum Ride, Ferris Wheel, Rollercoaster
        //                                      Zoo:             PL: Zoo Plaza, Zoo Cafe #1, Zoo Souvenir Shop #1, Zoo Restrooms #1, Moose And Reindeer Enclosure, Bird House, Antelope Enclosure, Bison Enclosure,
        //                                                           {Insect, Amphibian and Reptile House}, Flamingo Enclosure, Elephant Enclosure, Sealife Enclosure, Giraffe Enclosure, Monkey Palace, Rhino Enclosure, Lion Enclosure
        //                                      Nature Reserve:  PL: Campfire Site #1, Campfire Site #2, Tent #1, Tent #2, Tent #3, Viewing Deck #1, Viewing Deck #2, Tent Camping Site #1, Lean-To Shelter #1, Lean-To Shelter #2,
        //                                                           Lookout Tower #1, Lookout Tower #2, Camping Site #1, Fishing Cabin #1, Fishing Cabin #2, Hunting Cabin #1, Hunting Cabin #2, Bouldering Site #1
        // ParkGateAI                   GC      City Park:       PL: Park Main Gate, Small Park Main Gate, Park Side Gate
        //                                      Amusement Park:  PL: Amusement Park Main Gate, Small Amusement Park Main Gate, Amusement Park Side Gate
        //                                      Zoo:             PL: Zoo Main Gate, Small Zoo Main Gate, Zoo Side Gate
        //                                      Nature Reserve:  PL: Nature Reserve Main Gate, Small Nature Reserve Main Gate, Nature Reserve Side Gate
        // PoliceStationAI              GC      Police Station BG, Police Headquarters BG, Prison AD, Intelligence Agency HT (CCP)
        // PostOfficeAI                 GC  OC  Post Office IN, Post Sorting Facility IN
        // PowerPlantAI                 GC  OC  Coal Power Plant BG, Oil Power Plant BG, Nuclear Power Plant BG, Geothermal Power Plant GC, Ocean Thermal Energy Conversion Plant GC
        //                                      (unlimited coal/oil reserves so cannot compute storage)
        //    DamPowerHouseAI                   Hydro Power Plant BG
        //    FusionPowerPlantAI                Fusion Power Plant BG (monument)
        //    SolarPowerPlantAI                 Solar Power Plant BG, Solar Updraft Tower GC
        //    WindTurbineAI             GC      Wind Turbine BG, Advanced Wind Turbine BG, Wave Power Plant HT (CCP)
        // RadioMastAI                  GC      Short Radio Mast ND, Tall Radio Mast ND
        // SaunaAI                      GC      Sauna SF, Sports Hall and Gymnasium GC, Community Pool GC, Yoga Garden GC
        // SchoolAI                     GC      Elementary School BG, High School BG, University BG, Community School GC, Institute of Creative Arts GC, Modern Technology Institute GC, Faculty HT (CCP)
        //    CampusBuildingAI          GC      Trade School:   CA: Trade School Dormitory, Trade School Study Hall, Trade School Groundskeeping, Book Club, Trade School Outdoor Study, Trade School Gymnasium, Trade School Cafeteria,
        //                                                          Trade School Fountain, Trade School Library, IT Club, Trade School Commencement Office, Trade School Academic Statue 1, Trade School Auditorium, Trade School Laboratories,
        //                                                          Trade School Bookstore, Trade School Media Lab, Beach Volleyball Club, Trade School Academic Statue 2
        //                                      Liberal Arts:   CA: Liberal Arts Dormitory, Liberal Arts Study Hall, Liberal Arts Groundskeeping, Drama Club, Liberal Arts Outdoor Study, Liberal Arts Gymnasium, Liberal Arts Cafeteria,
        //                                                          Liberal Arts Fountain, Liberal Arts Library, Art Club, Liberal Arts Commencement Office, Liberal Arts Academic Statue 1, Liberal Arts Auditorium, Liberal Arts Laboratories,
        //                                                          Liberal Arts Bookstore, Liberal Arts Media Lab, Dance Club, Liberal Arts Academic Statue 2
        //                                      University:     CA: University Dormitory, University Study Hall, University Groundskeeping, Futsal Club, University Outdoor Study, University Gymnasium, University Cafeteria
        //                                                          University Fountain, University Library, Math Club, University Commencement Office, University Academic Statue 1, University Auditorium, University Laboratories,
        //                                                          University Bookstore, University Media Lab, Chess Club, University Academic Statue 2
        //       UniqueFacultyAI                Trade School:   CA: Police Academy, School of Tourism And Travel, School of Engineering
        //                                      Liberal Arts:   CA: School of Education, School of Environmental Studies, School of Economics
        //                                      University:     CA: School of Law, School of Medicine, School of Science
        // ShelterAI                    GC  OC  Small Emergency Shelter ND, Large Emergency Shelter ND
        // SnowDumpAI                   GC      Snow Dump SF
        // SpaceElevatorAI              GC      Space Elevator BG (monument)
        // SpaceRadarAI                 GC      Deep Space Radar ND
        // TaxiStandAI                  GC      Taxi Stand AD (taxis wait at a Taxi Stand for a customer, taxis are not generated by a Taxi Stand)
        // TollBoothAI                  GC      Two-Way Toll Booth BG, One-Way Toll Booth BG, Two-Way Large Toll Booth BG, One-Way Large Toll Booth BG
        // TourBuildingAI               GC      Hot Air Balloon Tours PL
        // TsunamiBuoyAI                GC      Tsunami Warning Buoy ND
        // WarehouseAI                  GC  OC  Forestry:  IN: Small Log Yard, Saw Dust Storage, Large Log Yard, Wood Chip Storage
        //                                      Farming:   IN: Small Grain Silo, Large Grain Silo, Small Barn, Large Barn
        //                                      Ore:       IN: Sand Storage, Ore Storage, Ore Industry Storage, Raw Mineral Storage
        //                                      Oil:       IN: Small Crude Oil Tank Farm, Large Crude Oil Tank Farm, Crude Oil Storage Cavern, Oil Industry Storage
        //                                      Generic:   IN: Warehouse Yard, Small Warehouse, Medium Warehouse, Large Warehouse
        // WaterCleanerAI               GC      Floating Garbage Collector GC
        // WaterFacilityAI              GC      Water Pumping Station BG, Water Tower BG, Large Water Tower SH, Water Drain Pipe BG, Water Treatment Plant BG,
        //                                      Inland Water Treatment Plant SH, Advanced Inland Water Treatment Plant SH, Eco Water Outlet GC, Eco Water Treatment Plant GC,
        //                                      Eco Inland Water Treatment Plant SH, Eco Advanced Inland Water Treatment Plant SH, Fresh Water Outlet ND
        // WaterFacilityAI              GC      Tank Reservoir ND
        // WaterFacilityAI              GC      Pumping Service ND
        // WeatherRadarAI               GC      Weather Radar ND




        /// <summary>
        /// create a patch for every building AI that has a GetColor method with logic for Outside Connections
        /// in the listings above, that is building AIs marked with GC and OC
        /// </summary>
        public static bool CreateGetColorPatches()
        {
            if (!CreateGetColorPatch<CommercialBuildingAI >()) return false;
            if (!CreateGetColorPatch<OfficeBuildingAI     >()) return false;
            if (!CreateGetColorPatch<IndustrialBuildingAI >()) return false;
            if (!CreateGetColorPatch<IndustrialExtractorAI>()) return false;

            if (!CreateGetColorPatch<FishFarmAI           >()) return false;
            if (!CreateGetColorPatch<FishingHarborAI      >()) return false;
            if (!CreateGetColorPatch<HeatingPlantAI       >()) return false;
            if (!CreateGetColorPatch<ExtractingFacilityAI >()) return false;
            if (!CreateGetColorPatch<ProcessingFacilityAI >()) return false;
            if (!CreateGetColorPatch<MarketAI             >()) return false;
            if (!CreateGetColorPatch<PostOfficeAI         >()) return false;
            if (!CreateGetColorPatch<PowerPlantAI         >()) return false;
            if (!CreateGetColorPatch<ShelterAI            >()) return false;
            if (!CreateGetColorPatch<WarehouseAI          >()) return false;

            // success
            return true;
        }

        /// <summary>
        /// create a patch of the GetColor method for the specified building AI type
        /// </summary>
        private static bool CreateGetColorPatch<T>() where T : CommonBuildingAI
        {
            // same routine is used for all building AI types
            return HarmonyPatcher.CreatePrefixPatch(typeof(T), "GetColor", BindingFlags.Instance | BindingFlags.Public, typeof(BuildingAIPatch), "BuildingAIGetColor");
        }

        /// <summary>
        /// return the color of the building
        /// same routine is used for all building AI types
        /// </summary>
        /// <returns>whether or not to do base processing</returns>
        public static bool BuildingAIGetColor(ushort buildingID, ref Building data, InfoManager.InfoMode infoMode, ref Color __result)
        {
            // do processing for this mod only for Outside Connections info view
            if (infoMode == InfoManager.InfoMode.Connections)
            {
                return EOCVUserInterface.instance.GetBuildingColor(buildingID, ref data, ref __result);
            }

            // do the base processing
            return true;
        }
    }
}
