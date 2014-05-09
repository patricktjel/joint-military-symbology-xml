﻿/* Copyright 2014 Esri
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace JointMilitarySymbologyLibrary
{
    public class ImageEntityExport : EntityExport, IEntityExport
    {
        // Class designed to implement the export of a SymbolSet and one entity from that SymbolSet
        // by outputting image file path information, its name and the category of icon it falls within,
        // and the tags associated with that SymbolSet and entity.

        public ImageEntityExport(ConfigHelper configHelper)
        {
            _configHelper = configHelper;
        }

        string IEntityExport.Headers
        {
            get { return "filePath,pointSize,styleItemName,styleItemCategory,styleItemTags,notes"; }
        }

        string IEntityExport.Line(SymbolSet ss, SymbolSetEntity e, SymbolSetEntityEntityType eType, SymbolSetEntityEntityTypeEntitySubType eSubType)
        {
            _notes = "";

            string result = "";
            string graphic = "";
            IconType iType = IconType.MAIN;

            string graphicPath = _configHelper.GetPath(ss.ID, FindEnum.FindEntities);
                         
            if (eSubType != null)
            {
                // TODO: handle this differently, but for now, we export the square
                // graphic if there are in fact multiple files.

                if (eSubType.Graphic != "" && eSubType.Icon != IconType.FULL_FRAME)
                    graphic = eSubType.Graphic;
                else
                    graphic = eSubType.SquareGraphic;

                iType = eSubType.Icon;
            }
            else if (eType != null)
            {
                if (eType.Graphic != "" && eType.Icon != IconType.FULL_FRAME)
                    graphic = eType.Graphic;
                else
                    graphic = eType.SquareGraphic;

                iType = eType.Icon;
            }
            else if (e != null)
            {
                if (e.Graphic != "" && e.Icon != IconType.FULL_FRAME)
                    graphic = e.Graphic;
                else
                    graphic = e.SquareGraphic;

                iType = e.Icon;
            }

            if (iType == IconType.NA)
                _notes = _notes + "icon is NA - entity is never to be drawn;";
            else
                _notes = _notes + "icon is " + Convert.ToString(iType) + ";";

            string itemRootedPath = _configHelper.BuildRootedPath(graphicPath, graphic);
            string itemActualPath = _configHelper.BuildActualPath(graphicPath, graphic);

            if (!File.Exists(itemActualPath))
                _notes = _notes + "image file does not exist;";
            
            string itemName = BuildEntityItemName(ss, e, eType, eSubType);
            string itemCategory = BuildEntityItemCategory(ss, iType);
            string itemTags = BuildEntityItemTags(ss, e, eType, eSubType);

            result = itemRootedPath + "," +
                     Convert.ToString(_configHelper.PointSize) + "," +
                     itemName + "," +
                     itemCategory + "," +
                     itemTags + "," +
                     _notes;

            return result;
        }
    }
}
