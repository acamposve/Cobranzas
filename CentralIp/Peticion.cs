using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CentralIp
{
    public interface Peticion
    {
        String ObtenerXML();
    }
    public class PetLogin : Peticion
    {
        public string UserName;
        public string Password;
        public string ObtenerXML()
        {
            String Request = "<login>";
            Request += "<username>" + UserName + "</username>";
            Request += "<password>" + Password + "</password>";
            Request += "</login>";
            return String.Format("<request id=\"{0}\">{1}</request>", 1, Request);
        }
    }
    public class PetLogout : Peticion
    {
        public string ObtenerXML()
        {
            String Request = "<logout>";
            Request += "</logout>";
            return String.Format("<request id=\"{0}\">{1}</request>", 1, Request);
        }
    }
    public class PetGetAgentStatus : Peticion
    {
        public string AgentNumber;
        public string ObtenerXML()
        {
            String Request = "<getagentstatus>";
            Request += "<agent_number>" + AgentNumber + "</agent_number>";
            Request += "</getagentstatus>";
            return String.Format("<request id=\"{0}\">{1}</request>", 1, Request);
        }
    }
    public class PetLoginAgent : Peticion
    {
        public string AgentNumber;
        public string AgentHash;
        public string Extension;
        public string Password;
        public string ObtenerXML()
        {
            String Request = "<loginagent>";
            Request += "<agent_number>" + AgentNumber + "</agent_number>";
            Request += "<agent_hash>" + AgentHash + "</agent_hash>";
            Request += "<extension>" + Extension + "</extension>";
            if (Password != "")
                Request += "<password>" + Password + "</password>";
            Request += "</loginagent>";
            return String.Format("<request id=\"{0}\">{1}</request>", 1, Request);
        }
    }
    public class PetLogoutAgent : Peticion
    {
        public string AgentNumber;
        public string AgentHash;
        public string ObtenerXML()
        {
            String Request = "<logoutagent>";
            Request += "<agent_number>" + AgentNumber + "</agent_number>";
            Request += "<agent_hash>" + AgentHash + "</agent_hash>";
            Request += "</logoutagent>";
            return String.Format("<request id=\"{0}\">{1}</request>", 1, Request);
        }
    }
}
