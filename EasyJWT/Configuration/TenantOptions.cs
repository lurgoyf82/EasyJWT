using System.Collections.Generic;
using EasyJWT.Keys;

namespace EasyJWT.Configuration
{
    /// <summary>
    /// Configurazione specifica per ciascun tenant.
    /// </summary>
    public sealed class TenantOptions
    {
        /// <summary>
        /// Issuer (claim "iss") da usare per questo tenant.
        /// </summary>
        public string Issuer { get; set; } = null!;

        /// <summary>
        /// Audience possibili per questo tenant.
        /// </summary>
        public IList<string> Audiences { get; } = new List<string>();

        /// <summary>
        /// Elenco delle policy di token abilitate per questo tenant (nomi di TokenPolicyOptions).
        /// </summary>
        public ISet<string> AllowedTokenPolicies { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Provider di chiavi override per questo tenant (se non impostato, si usa quello globale).
        /// </summary>
        public IKeyProvider? KeyProvider { get; set; }
    }
}
