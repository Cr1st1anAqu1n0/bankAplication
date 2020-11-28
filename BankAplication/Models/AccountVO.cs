using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankAplication.Models
{
    public class AccountVO
    {
        /// <summary>
        /// codigo del departamento donde vive
        /// </summary>
        public int IdDepartament { get; set; }
        /// <summary>
        /// codigo del cliente unico
        /// </summary>
        public decimal IdClient { get; set; }
        /// <summary>
        /// saldo de la cuenta al iniciar la creacion
        /// </summary>
        public decimal Balance { get; set; }
        /// <summary>
        /// codigo de moneda de la transaccion
        /// </summary>
        public decimal IdMoneda { get; set; }
    }
}