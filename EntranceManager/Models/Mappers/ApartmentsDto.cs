﻿namespace EntranceManager.Models.Mappers
{
    public class ApartmentDto
    {
        public int Floor { get; set; }
        public int Number { get; set; }
        public int OwnerUserId { get; set; }
        public int EntranceId { get; set; }
        public int NumberOfChildren { get; set; }
        public int NumberOfPets { get; set; }
    }
}
