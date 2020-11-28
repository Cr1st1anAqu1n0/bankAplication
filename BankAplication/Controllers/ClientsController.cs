using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BankAplication.Controllers
{
    public class ClientsController : ApiController
    {
        [Route("api/clientAll")]
        [HttpGet]
        // GET: api/Clients
        public IHttpActionResult Get()
        {
            try
            {
                using (ModelEF.dbbankaplicationEntitiesBanck context = new ModelEF.dbbankaplicationEntitiesBanck())
                {
                    return Ok(context.CLIENTs.ToList());
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
            
        }

       

        [Route("api/clients/create")]
        [HttpPost]
        public IHttpActionResult SaveClient([FromBody]Models.ClientVO model)
        {
            try
            {
                ModelEF.CLIENT clietn = new ModelEF.CLIENT
                {
                    codcliente = model.Id,
                    name = model.Nombre
                };

                using (ModelEF.dbbankaplicationEntitiesBanck context = new ModelEF.dbbankaplicationEntitiesBanck())
                {
                    using (var bdContextTransaction = context.Database.BeginTransaction())
                    {
                        context.Entry(clietn).State = System.Data.Entity.EntityState.Added;
                        context.SaveChanges();
                        bdContextTransaction.Commit();
                    }
                }

                return Ok(model.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

      
    }
}
