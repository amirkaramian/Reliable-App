using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAuto
{
    public class Automation
    {
        // Custom Fields To be field in by user
        // must be a valid json string
        public string CustomFields { get; set; } = @"{ 'inputFields': [ { 'name': 'title', 'type': 'text', 'label': 'Test Field', 'placeholder': 'Enter Title Here', 'defaultValue': 'test default' }, { 'name': 'brandIds', 'type': 'select', 'label': 'Brands', 'placeholder': 'All Brands', 'options': [{ 'label': 'brand test', 'value': 'test', }], }, ] }";

        // Function for suspend automation
        public Task<object> OnSuspend(object data, dynamic localApi)
        {
            return Task.FromResult(data);
        }

        // Function for create automation
        public Task<object> OnCreate(object data, dynamic localApi)
        {
            return Task.FromResult(data);
        }

        // Function for renew automation
        public Task<object> OnRenew(object data, dynamic localApi)
        {
            return Task.FromResult(data);
        }

        // Function for terminate automation
        public Task<object> OnTerminate(object data, dynamic localApi)
        {
            return Task.FromResult(data);
        }
    }
}
