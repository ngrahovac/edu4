import React from 'react'
import { SectionTitle } from '../../layout/SectionTitle'
import PrimaryButton from '../buttons/PrimaryButton'

const CTA = () => {
    return (
        <div className='flex flex-row align-middle place-items-center place-items gap-x-2 w-1/3'>
            <div className='flex flex-col items-center space-y-4 w-full'>
                <p className='w-full text-left font-semibold text-lg md:text-xl text-gray-700'>Stay in the Loop</p>
                <p className='text-gray-600 text-left w-full'>Subscribe to our newsletter and receive updates on platform development and early access to public beta.</p>
                <form
                    onSubmit={e => e.preventDefault()}
                    className='flex flex-row rounded-full border-2 border-gray-400 justify-between px-2 py-1 w-full focus-within:border-2 focus-within:border-indigo-500'>
                    <input
                        type="email"
                        required={true}
                        name="title"
                        placeholder='Your email here..'
                        className="bg-transparent md:text-lg border-transparent focus:border-transparent focus:ring-0 caret-indigo-500">
                    </input>
                    <button
                        className='md:text-lg font-semibold px-4 rounded-full text-indigo-500 hover:text-indigo-600 cursor-pointer'>
                        Subscribe
                    </button>
                </form>
            </div>

        </div>
    )
}

export default CTA