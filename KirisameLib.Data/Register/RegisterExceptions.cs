﻿using System.Diagnostics;

namespace KirisameLib.Data.Register;

public class GettingFallbackValueFailedException(string id, Exception inner) : Exception($"Failed to get fallback value for item: ID: {id}", inner);