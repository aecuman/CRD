using CRD.Application.Common.ViewModels;
using CRD.Application.StructureCategories.Commands;
using CRD.Application.StructureCategories.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRD.API.Controllers
{
    [Route("api/structure-categories")]
    public class StructureCategoryController : BaseController
    {
        [HttpPost]
        public async Task<ActionResult<int>> Create(CreateStructureCategoryCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPut]
        public async Task<ActionResult<bool>> Update(UpdateStructureCategoryCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return await Mediator.Send(new DeleteStructureCategoryCommand { Id = id });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StructureCategoryViewModel>>> GetAll()
        {
            return Ok( await Mediator.Send(new GetAllStructureCategoriesQuery()));
        }
    }
}
