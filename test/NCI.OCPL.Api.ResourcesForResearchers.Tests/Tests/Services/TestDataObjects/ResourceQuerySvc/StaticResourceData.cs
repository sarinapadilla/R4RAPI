using System;
using NCI.OCPL.Api.ResourcesForResearchers.Models;

namespace NCI.OCPL.Api.ResourcesForResearchers.Tests.Services
{
    public static class StaticResourceData
    {
        public static Resource GetRes101() {
            return new Resource
            {
                ID = 101,
                Title = "NCI Data Catalog",
                Website = "https://www.cancer.gov/research/resources/data-catalog",
                ResourceAccess = new ResourceAccess { Type = "open" },
                ToolTypes = new KeyLabel[] {
                    new KeyLabel{ Label = "Datasets & Databases", Key = "datasets_databases"}
                },
                ToolSubtypes = new ToolSubtype[] {
                    new ToolSubtype{ Label = "Clinical Data", Key = "clinical_data", ParentKey = "datasets_databases" },
                    new ToolSubtype{ Label = "Epidemiologic Data", Key = "epidemiologic_data", ParentKey = "datasets_databases" },
                    new ToolSubtype{ Label = "Genomic Datasets", Key = "genomic_datasets", ParentKey = "datasets_databases" },
                    new ToolSubtype{ Label = "Imaging", Key = "imaging", ParentKey = "datasets_databases" }
                },
                ResearchAreas = new KeyLabel[] {
                    new KeyLabel{ Key = "cancer_biology",Label = "Cancer Biology" },
                    new KeyLabel{ Key = "cancer_omics", Label = "Cancer Omics" },
                    new KeyLabel{ Key = "cancer_survivorship", Label = "Cancer Survivorship" },
                    new KeyLabel{ Key = "screening_detection", Label = "Screening & Detection" },
                    new KeyLabel{ Key = "cancer_treatment", Label = "Cancer Treatment" }
                },
                ResearchTypes = new KeyLabel[] {
                    new KeyLabel{ Key = "basic", Label = "Basic" },
                    new KeyLabel{ Key = "clinical", Label = "Clinical" },
                    new KeyLabel{ Key = "epidemiologic", Label = "Epidemiologic" },
                    new KeyLabel{ Key = "translational", Label = "Translational" }
                },
                DOCs = new KeyLabel[] {
                    new KeyLabel { Key = "cbiit", Label = "Center for Biomedical Informatics and Informatics Technology (CBIIT)" }
                },
                POCs = new Contact[] {
                    new Contact{
                        Name = new Name {
                            FirstName = "Mervi",
                            LastName = "Heiskanen"
                        },
                        Email = "Mervi.Heiskanen@nih.gov",
                        Phone = "240-276-5175",
                        Title = "Program Manager NCI CBIIT"
                    }
                },
                Description = "<p>The NCI Data Catalog is a listing of data collections produced by major NCI initiatives and other widely used data sets. Data collections included in the catalog meet the following criteria: Produced by NCI intramural researchers or major NCI initiatives, regularly referenced NCI funded extramural research data, available to all researchers and may be Open or Controlled Access (requiring approval by a Data Access Committee), well-documented and available for download.</p>"
            };
        }
    }
}
