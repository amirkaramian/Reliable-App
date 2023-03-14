using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Settings;
public class UpdateAutoBillUserAppSettingRequest : IMustBeValid
{
    public bool AutoBill { get; set; }
}
