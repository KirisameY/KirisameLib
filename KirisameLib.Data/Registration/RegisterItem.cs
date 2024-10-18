using System.Data.Common;

namespace KirisameLib.Data.Registration;

public delegate bool RegisterItem<in TItem>(string id, TItem item);