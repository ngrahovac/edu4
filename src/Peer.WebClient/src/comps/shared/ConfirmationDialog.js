import React from 'react'
import NeutralButton from '../buttons/NeutralButton';
import PrimaryButton from '../buttons/PrimaryButton';
import BorderlessButton from '../buttons/BorderlessButton';

const ConfirmationDialog = (props) => {

    const {
        question,
        description,
        onConfirm,
        onCancel
    } = props;

    return (
        <div>
            <div className='relative p-8 rounded-xl bg-white'>
                <div className='mb-24'>
                    <p className='font-bold text-2xl mb-4'>{question}</p>
                    <p className='text-justify'>{description}</p>
                </div>

                <div className='absolute flex flex-row space-x-2 bottom-8 right-8'>
                    <BorderlessButton
                        text="Go back"
                        onClick={onCancel}>
                    </BorderlessButton>
                    
                    <PrimaryButton
                        text="Confirm"
                        onClick={onConfirm}>
                    </PrimaryButton>
                </div>
            </div>
        </div>
    )
}

export default ConfirmationDialog