using System;
using R4RAPI.Models;

namespace R4RAPI.Test.Services
{
    public static class StaticResourceData
    {
        public static Resource GetRes101() {
            return new Resource
            {
                ID = 101,
                Title = "NCI Data Catalog",
                Website = "https://www.cancer.gov/research/resources/data-catalog",
                ToolTypes = new KeyLabel[] {
                    new KeyLabel{ Label = "Datasets & Databases", Key = "datasets_databases"}
                },
                ResearchAreas = new KeyLabel[] {
                    new KeyLabel{ Key = "cancer_biology",Label = "Cancer Biology" },
                    new KeyLabel{ Key = "cancer_omics", Label = "Cancer Omics" },
                    new KeyLabel{ Key = "cancer_survivorship", Label = "Cancer Survivorship" },
                    new KeyLabel{ Key = "screening_detection", Label = "Screening & Detection" },
                    new KeyLabel{ Key = "cancer_treatment", Label = "Cancer Treatment" }
                },

            };

            /*
{
    "researchTypes": [
      {
        "key": "basic",
        "label": "Basic"
      },
      {
        "key": "clinical",
        "label": "Clinical"
      },
      {
        "key": "epidemiologic",
        "label": "Epidemiologic"
      },
      {
        "key": "translational",
        "label": "Translational"
      }
    ],
    "resourceAccess": { "type": "open" },
    "docs": [
      {
        "key": "cbiit",
        "label": "Center for Biomedical Informatics and Informatics Technology (CBIIT)"
      }
    ],
    "body": "<p>The NCI Data Catalog is a listing of data collections produced by major NCI initiatives and other widely used data sets. Data collections included in the catalog meet the following criteria: Produced by NCI intramural researchers or major NCI initiatives, regularly referenced NCI funded extramural research data, available to all researchers and may be Open or Controlled Access (requiring approval by a Data Access Committee), well-documented and available for download.</p>",
    "description": "",
    "pocs": [
      {
        "email": "Mervi.Heiskanen@nih.gov",
        "name": {
          "firstname": "Mervi",
          "lastname": "Heiskanen"
        },
        "phone": "240-276-5175",
        "title": "Program Manager NCI CBIIT"
      }
    ],
    "toolSubtypes": [
      {
        "label": "Clinical Data",
        "key": "clinical_data",
        "parentKey": "datasets_databases"
      },
      {
        "label": "Epidemiologic Data",
        "key": "epidemiologic_data",
        "parentKey": "datasets_databases"
      },
      {
        "label": "Genomic Datasets",
        "key": "genomic_datasets",
        "parentKey": "datasets_databases"
      },
      {
        "label": "Imaging",
        "key": "imaging",
        "parentKey": "datasets_databases"
      }
    ]
  }
}              
             
             */ 

        }
    }
}
