import React from 'react'

const UserActionFailToast = (props) => {
    const { caption } = props;

    return (
        <dialog
            id='user-action-fail-toast'
            className='fixed z-50 m-0 w-min-64 top-4 right-4 left-auto stroke-1 stroke-red-500 bg-red-100 border shadow-sm justify text-md text-slate-600 rounded-md px-6'>

            <form
                method="dialog"
                className='inline-flex flex-row shrink-0 items-center'>
                <p
                    className='mb-1 '>
                    {caption}
                </p>

                <button
                    type="button"
                    onClick={() => { document.getElementById('user-action-fail-toast').close() }}
                    className='ml-10'>
                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="w-6 h-6 stroke-slate-400 hover:stroke-red-500">
                        <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
                    </svg>
                </button>
            </form>
        </dialog>
    )
}

export default UserActionFailToast