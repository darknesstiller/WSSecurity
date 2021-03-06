﻿using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace CertFixEscapedComma
{
    public class CertRefEncodingBindingElement : MessageEncodingBindingElement
    {
        private MessageEncodingBindingElement innerMsgEncBindElmt;
        private MessageEncoder enc;

        public CertRefEncodingBindingElement()
            : this(new TextMessageEncodingBindingElement(), null)
        {
        }

        //public CertRefEncodingBindingElement(MessageEncodingBindingElement innerEBE) : this(innerEBE, null)
        //{
        //}

        public CertRefEncodingBindingElement(MessageEncodingBindingElement innerEBE, MessageEncoder enc)
        {
            innerMsgEncBindElmt = innerEBE;
            if (innerMsgEncBindElmt != null)
                innerMsgEncBindElmt.MessageVersion = MessageVersion.Soap11;
            this.enc = enc;
        }

        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            CertRefEncoderFactory factory = new CertRefEncoderFactory(innerMsgEncBindElmt.CreateMessageEncoderFactory(), enc);
            return factory;
        }

        public override MessageVersion MessageVersion
        {
            get
            {
                return innerMsgEncBindElmt?.MessageVersion;
            }
            set
            {
                if (innerMsgEncBindElmt != null)
                    innerMsgEncBindElmt.MessageVersion = value;
            }
        }

        public override BindingElement Clone()
        {
            CertRefEncodingBindingElement newBE = null;
            if (innerMsgEncBindElmt != null)
                newBE = new CertRefEncodingBindingElement((MessageEncodingBindingElement)innerMsgEncBindElmt.Clone(), enc);
            else
                newBE = new CertRefEncodingBindingElement();
            return newBE;
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("Context");
            return context.CanBuildInnerChannelFactory<TChannel>();
        }


        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("Context");
            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }


        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("Context");

            return context.CanBuildInnerChannelFactory<TChannel>();
        }


        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("Context");
            context.BindingParameters.Add(this);
            return context.BuildInnerChannelListener<TChannel>();
        }
    }


    public class CertRefBEExtentionElement : BindingElementExtensionElement
    {
        public override Type BindingElementType
        {
            get { return typeof(CertRefEncodingBindingElement); }
        }

        protected override BindingElement CreateBindingElement()
        {
            return new CertRefEncodingBindingElement();
        }
    }

}
