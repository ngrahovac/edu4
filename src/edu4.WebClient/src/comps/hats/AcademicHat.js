import React from 'react'
import HatParam from './HatParam'
import { ClipboardDocumentListIcon } from '@heroicons/react/24/outline';

const AcademicHat = (props) => {
    const { hat } = props;

    return (
        <div>
            <p
                className='text-slate-800 text-sm mb-2 font-semibold'>
                Academic / Researcher
            </p>

            <div
                className='flex flex-col'>
                <HatParam
                    text={hat.parameters.researchField}
                    icon={<ClipboardDocumentListIcon className="w-6 h-6 mr-2 fill-stone-100 stroke-slate-800 stroke-1"></ClipboardDocumentListIcon>}>
                </HatParam>
            </div>
        </div>
    )
}

export default AcademicHat