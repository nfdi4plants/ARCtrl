from __future__ import annotations
from collections.abc import Callable
from typing import Any
# Unions are transpiled into an implementation class holding the members and a
# type alias over the case classes. The public name has to be the implementation
# class, otherwise members and `isinstance` are unavailable. See ARCtrl/__init__.py.
from .py.CWL.cwlprocessing_unit import _CWLProcessingUnit as CWLProcessingUnit
from .py.CWL.cwltypes import _CWLType as CWLType, _SchemaSaladString as SchemaSaladString, FileInstance, DirectoryInstance, DirentInstance, InputEnumSchema, InputRecordField, InputRecordSchema, InputArraySchema, SchemaDefRequirementType, SoftwarePackage
from .py.CWL.parameter_value import _CWLParameterValue as CWLParameterValue, CWLParameterRecordField
from .py.CWL.parameter_reference import CWLParameterReference
from .py.CWL.inputs import CWLInput, InputBinding
from .py.CWL.outputs import CWLOutput, OutputBinding, _OutputSource as OutputSource
from .py.CWL.tool_description import CWLToolDescription
from .py.CWL.workflow_description import CWLWorkflowDescription
from .py.CWL.expression_tool_description import CWLExpressionToolDescription
from .py.CWL.operation_description import CWLOperationDescription
from .py.CWL.workflow_steps import (
    WorkflowStep, StepInput, _StepOutput as StepOutput, StepOutputParameter,
    _WorkflowStepRun as WorkflowStepRun, _LinkMergeMethod as LinkMergeMethod,
    _PickValueMethod as PickValueMethod, _ScatterMethod as ScatterMethod,
)
from .py.CWL.requirements import (
    _Requirement as Requirement, DockerRequirement, EnvironmentDef, ResourceRequirementInstance,
    _HintEntry as HintEntry, HintUnknownValue, _InitialWorkDirEntry as InitialWorkDirEntry,
    InlineJavascriptRequirementValue, InplaceUpdateRequirementValue,
    _LoadListingEnum as LoadListingEnum, LoadListingRequirementValue,
    NetworkAccessRequirementValue, _ToolTimeLimitValue as ToolTimeLimitValue,
    WorkReuseRequirementValue,
)
