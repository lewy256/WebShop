using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BasketApi.IntegrationTests {
    public class FakePolicyEvaluator : IPolicyEvaluator {
        public Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context) {
            var claimsPrincipal = new ClaimsPrincipal();

            claimsPrincipal.AddIdentity(new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, "39b74f0a-b286-4d7a-bdfd-56c81da8b895"),
                new Claim(ClaimTypes.Name, "lewy256"),
                new Claim(ClaimTypes.Role, "Administrator")
            ]));

            var ticket = new AuthenticationTicket(claimsPrincipal, "Test");
            var result = AuthenticateResult.Success(ticket);
            return Task.FromResult(result);
        }

        public Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy, AuthenticateResult authenticationResult, HttpContext context, object resource) {
            var result = PolicyAuthorizationResult.Success();
            return Task.FromResult(result);
        }
    }
}
