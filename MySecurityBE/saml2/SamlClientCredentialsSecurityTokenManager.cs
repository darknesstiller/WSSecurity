﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace MySecurityBE.Saml2
{

    //A REVISAR https://msdn.microsoft.com/en-us/library/aa355062(v=vs.110).aspx
    public class SamlAssertiondClientCredentialsSecurityTokenManager : ClientCredentialsSecurityTokenManager
    {
        SamlAssertionClientCredentials samlAssertionClientCredentials;

        public SamlAssertiondClientCredentialsSecurityTokenManager(SamlAssertionClientCredentials creditCardClientCredentials)
            : base(creditCardClientCredentials)
        {
            this.samlAssertionClientCredentials = creditCardClientCredentials;
        }

        public override SecurityTokenProvider CreateSecurityTokenProvider(SecurityTokenRequirement tokenRequirement)
        {
            if (tokenRequirement.TokenType == "urn:oasis:names:tc:SAML:2.0:assertion")
            {
                return new SamlTokenProvider(this.samlAssertionClientCredentials.SamlInfo);
            }
            else if (tokenRequirement is InitiatorServiceModelSecurityTokenRequirement)
            {
                if (tokenRequirement.TokenType == SecurityTokenTypes.X509Certificate)
                {
                    MessageDirection direction = tokenRequirement.GetProperty<MessageDirection>(ServiceModelSecurityTokenRequirement.MessageDirectionProperty);
                    if (direction == MessageDirection.Output)
                    {

                        if (tokenRequirement.KeyUsage == SecurityKeyUsage.Signature)
                            return new X509SecurityTokenProvider(samlAssertionClientCredentials.ServiceCertificate.DefaultCertificate);
                        else
                            return new X509SecurityTokenProvider(samlAssertionClientCredentials.ClientCertificate.Certificate);
                    }
                }
            }
            else if (tokenRequirement is RecipientServiceModelSecurityTokenRequirement)
            {
                if (tokenRequirement.TokenType == SecurityTokenTypes.X509Certificate)
                {
                    MessageDirection direction = tokenRequirement.GetProperty<MessageDirection>(ServiceModelSecurityTokenRequirement.MessageDirectionProperty);
                    if (direction == MessageDirection.Input)
                    {

                        if (tokenRequirement.KeyUsage == SecurityKeyUsage.Signature)
                            return new X509SecurityTokenProvider(samlAssertionClientCredentials.ServiceCertificate.DefaultCertificate);
                        else
                            return new X509SecurityTokenProvider(samlAssertionClientCredentials.ClientCertificate.Certificate);
                    }
                }
            }
            return base.CreateSecurityTokenProvider(tokenRequirement);
        }

        public override SecurityTokenSerializer CreateSecurityTokenSerializer(SecurityTokenVersion version)
        {
            return new SamlAssertionSecurityTokenSerializer(version);
        }

    }
}
