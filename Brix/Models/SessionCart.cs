using Brix.Infrastructure;
using Brix.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http; // Required for session management
using System.Text.Json; // Required for JSON serialization

namespace Brix.Models
{
    public class SessionCart : Cart
    {
        public static Cart GetCart(IServiceProvider services)
        {
            ISession? session = services.GetRequiredService<IHttpContextAccessor>()
                .HttpContext?.Session;
            SessionCart cart = session?.GetJson<SessionCart>("Cart") ?? new SessionCart();

            cart.Session = session;
            return cart;
        }

        [JsonIgnore]
        public ISession? Session { get; set; }

        public override void AddItem(Product prod, int quantity)
        {
            base.AddItem(prod, quantity);
            Session?.SetJson("Cart", this);
        }

        public override void RemoveLine(Product prod)
        {
            base.RemoveLine(prod);
            Session?.SetJson("Cart", this);
        }

        public override void Clear()
        {
            base.Clear();
            Session?.SetJson("Cart", this);
        }
    }
}

// You can place the following class in the same file or a separate one as needed.
public static class SessionExtensions
{
    public static void SetJson(this ISession session, string key, object value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    public static T GetJson<T>(this ISession session, string key)
    {
        var sessionData = session.GetString(key);
        return sessionData == null ? default : JsonSerializer.Deserialize<T>(sessionData);
    }
}
