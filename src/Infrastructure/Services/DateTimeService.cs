﻿using CRM.App.Application.Common.Interfaces;

namespace CRM.App.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}
