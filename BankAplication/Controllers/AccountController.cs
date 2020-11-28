using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BankAplication.Controllers
{
    public class AccountController : ApiController
    {
        [Route("api/account")]
        [HttpGet]
        // GET: api/Account
        public IHttpActionResult GetAccount()
        {

            //nrocuenta, departamento, nombre t, moneda, saldo
            using (ModelEF.dbbankaplicationEntitiesBanck context = new ModelEF.dbbankaplicationEntitiesBanck())
            {
                return Ok(context.ACCOUNTs.Join(context.CLIENTs,
                                acc => acc.codclient,
                                cli => cli.codcliente,
                                (acc, cli) => new { acc, cli})
                                .Join(context.DEPARTAMENTs,
                                     conbinedEntry => conbinedEntry.acc.coddept,
                                     dep => dep.iddept,
                                     (conbinedEntry, dep) => new
                                     {
                                         cuenta = conbinedEntry.acc.accountnum,
                                         departamento = dep.description,
                                         nombreCliente = conbinedEntry.cli.name,
                                         moneda = (conbinedEntry.acc.coin == 1)?"BOL":"USD",
                                         saldo = conbinedEntry.acc.balance
                                     }
                                ).ToList()
                    );
            }
           
        }

        [Route("api/accountAll")]
        [HttpGet]
        // GET: api/Account
        public IHttpActionResult GetAccountAll()
        {

            //nrocuenta, departamento, nombre t, moneda, saldo
            using (ModelEF.dbbankaplicationEntitiesBanck context = new ModelEF.dbbankaplicationEntitiesBanck())
            {
                return Ok(context.ACCOUNTs.Select(x => x.accountnum).ToList());
            }

        }

        // GET: api/Account/5
        public string Get(int id)
        {
            return "value";
        }

        [Route("api/account/create")]
        [HttpPost]
        // POST: api/Account
        public IHttpActionResult CreateAccount([FromBody]Models.AccountVO model)
        {
            try
            {
                using (ModelEF.dbbankaplicationEntitiesBanck context = new ModelEF.dbbankaplicationEntitiesBanck())
                {
                    var moneda = context.COINs.Where(x => x.idcoin == model.IdMoneda).FirstOrDefault();

                    var depart = context.DEPARTAMENTs.Where(x => x.iddept == model.IdDepartament).FirstOrDefault();

                    var correlativo = context.ACCOUNTs.OrderBy(x => x.correlative).FirstOrDefault();

                    int correlativeNext = int.Parse(correlativo.correlative)+1;

                    string cuenta = moneda.codcoin + "-" + depart.coddept + "-"+ correlativeNext.ToString().PadLeft(6,'0');


                    ModelEF.ACCOUNT account = new ModelEF.ACCOUNT {
                        accountnum = cuenta,
                        balance = model.Balance,
                        codclient = model.IdClient,
                        coddept = model.IdDepartament,
                        correlative = correlativeNext.ToString().PadLeft(6,'0')     ,
                        states = true,
                        coin = moneda.idcoin                  
                    };

                        using (var bdContextTransaction = context.Database.BeginTransaction())
                        {
                            context.Entry(account).State = System.Data.Entity.EntityState.Added;
                            context.SaveChanges();
                            bdContextTransaction.Commit();
                        }


                    return Ok(cuenta);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("api/account/update")]
        [HttpPost]
        public IHttpActionResult ModificarCuenta(int id, [FromBody]Models.AccounInsertVO model)
        {
            try
            {
                using (ModelEF.dbbankaplicationEntitiesBanck context = new ModelEF.dbbankaplicationEntitiesBanck())
                {

                    var mov = context.MOVEMENTs.Where(x => x.accountnum.Trim() == model.accountnum.Trim()).Count();
                    if (mov == 0)
                    {
                        var account = context.ACCOUNTs.Where(x => x.accountnum.Trim() == model.accountnum.Trim()).FirstOrDefault();

                        if (account == null)
                        {
                            return NotFound();
                        }

                        account.balance = model.balance;
                        account.codclient = model.codclient;
                        account.coddept = model.coddept;
                        using (var bdContextTransaction = context.Database.BeginTransaction())
                        {
                            context.Entry(account).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            bdContextTransaction.Commit();
                        }
                        return Ok(account.accountnum);
                    }
                    else
                    {
                        return BadRequest("tiene movimientos en la cuenta");
                    }

                    
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("api/account/delete")]
        [HttpPost]
        public IHttpActionResult BorrarCuenta(int id, [FromBody]string number)
        {
            try
            {
                using (ModelEF.dbbankaplicationEntitiesBanck context = new ModelEF.dbbankaplicationEntitiesBanck())
                {

                      var account = context.ACCOUNTs.Where(x => x.accountnum.Trim() == number.Trim()).FirstOrDefault();



                        if (account == null)
                        {
                            return NotFound();
                        }
                    account.states = false;
                        using (var bdContextTransaction = context.Database.BeginTransaction())
                        {
                            context.Entry(account).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();
                            bdContextTransaction.Commit();
                        }
                        return Ok(account.accountnum);
                   


                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
